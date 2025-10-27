using CoinPay.Api.Data;
using CoinPay.Api.Models;
using CoinPay.Api.Services.Circle;
using CoinPay.Api.Services.Circle.Models;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Services.Auth;

/// <summary>
/// Authentication service implementation for passkey-based user registration and login
/// </summary>
public class AuthService : IAuthService
{
    private readonly ICircleService _circleService;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ICircleService circleService,
        AppDbContext dbContext,
        ILogger<AuthService> logger)
    {
        _circleService = circleService;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<RegistrationChallengeResponse> InitiateRegistrationAsync(string username)
    {
        _logger.LogInformation("Initiating registration for username: {Username}", username);

        // Check if username already exists
        if (await UsernameExistsAsync(username))
        {
            throw new InvalidOperationException($"Username '{username}' is already taken");
        }

        // Call Circle SDK to initiate registration
        var circleChallenge = await _circleService.InitiateUserRegistrationAsync(username);

        // Generate a local challenge ID for tracking
        var challengeId = Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Registration challenge created for username: {Username}, ChallengeId: {ChallengeId}",
            username,
            challengeId);

        return new RegistrationChallengeResponse
        {
            ChallengeId = challengeId,
            Challenge = circleChallenge.Challenge,
            UserId = circleChallenge.UserId
        };
    }

    /// <inheritdoc/>
    public async Task<UserRegistrationResponse> CompleteRegistrationAsync(CompleteRegistrationRequest request)
    {
        _logger.LogInformation("Completing registration for username: {Username}", request.Username);

        // Check if username already exists (in case of race condition)
        if (await UsernameExistsAsync(request.Username))
        {
            throw new InvalidOperationException($"Username '{request.Username}' is already taken");
        }

        // Complete registration with Circle SDK
        var circleRequest = new CircleRegistrationRequest
        {
            ChallengeId = request.ChallengeId,
            Username = request.Username,
            CredentialId = request.CredentialId,
            PublicKey = request.PublicKey,
            AttestationObject = request.AuthenticatorData,
            ClientDataJson = "{}" // Placeholder - should be sent from frontend
        };

        var circleUser = await _circleService.CompleteUserRegistrationAsync(circleRequest);

        // Create user in our database
        var user = new User
        {
            Username = request.Username,
            CircleUserId = circleUser.UserId,
            CredentialId = request.CredentialId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "User registered successfully. UserId: {UserId}, CircleUserId: {CircleUserId}",
            user.Id,
            user.CircleUserId);

        return new UserRegistrationResponse
        {
            UserId = user.Id,
            Username = user.Username,
            CircleUserId = user.CircleUserId,
            Message = "User registered successfully"
        };
    }

    /// <inheritdoc/>
    public async Task<LoginChallengeResponse> InitiateLoginAsync(string username)
    {
        _logger.LogInformation("Initiating login for username: {Username}", username);

        // Verify user exists
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            _logger.LogWarning("Login attempt for non-existent username: {Username}", username);
            throw new InvalidOperationException($"User '{username}' not found");
        }

        // Call Circle SDK to initiate authentication
        var circleChallenge = await _circleService.InitiateUserLoginAsync(username);

        // Generate a local challenge ID for tracking
        var challengeId = Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Login challenge created for username: {Username}, ChallengeId: {ChallengeId}",
            username,
            challengeId);

        return new LoginChallengeResponse
        {
            ChallengeId = challengeId,
            Challenge = circleChallenge.Challenge
        };
    }

    /// <inheritdoc/>
    public async Task<LoginResponse> CompleteLoginAsync(CompleteLoginRequest request)
    {
        _logger.LogInformation("Completing login for username: {Username}", request.Username);

        // Verify user exists
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            throw new InvalidOperationException($"User '{request.Username}' not found");
        }

        // Complete authentication with Circle SDK
        var circleRequest = new CircleAuthenticationRequest
        {
            ChallengeId = request.ChallengeId,
            Username = request.Username,
            CredentialId = request.CredentialId,
            AuthenticatorData = request.AuthenticatorData,
            Signature = request.Signature
        };

        var circleAuth = await _circleService.CompleteUserLoginAsync(circleRequest);

        // Update last login timestamp
        user.LastLoginAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        // Generate JWT token (simplified - in production, use proper JWT generation)
        var token = GenerateSimpleToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        _logger.LogInformation(
            "User logged in successfully. UserId: {UserId}, Username: {Username}",
            user.Id,
            user.Username);

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            WalletAddress = user.WalletAddress,
            ExpiresAt = expiresAt
        };
    }

    /// <inheritdoc/>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbContext.Users.AnyAsync(u => u.Username == username);
    }

    /// <summary>
    /// Generates a simple token for authentication
    /// NOTE: In production, use proper JWT with signing and claims
    /// </summary>
    private string GenerateSimpleToken(User user)
    {
        // Simplified token generation - in production, use System.IdentityModel.Tokens.Jwt
        var tokenData = $"{user.Id}:{user.Username}:{DateTime.UtcNow.Ticks}";
        var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenData);
        return Convert.ToBase64String(tokenBytes);
    }
}
