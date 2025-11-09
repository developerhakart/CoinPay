using Xunit;
using Moq;
using FluentAssertions;
using CoinPay.Api.Services.Auth;
using CoinPay.Api.Services.Circle;
using CoinPay.Api.Services.Circle.Models;
using CoinPay.Api.Data;
using CoinPay.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoinPay.Api.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly Mock<ICircleService> _mockCircleService;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AppDbContext _dbContext;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockCircleService = new Mock<ICircleService>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        _authService = new AuthService(
            _mockCircleService.Object,
            _dbContext,
            _mockLogger.Object,
            _mockJwtTokenService.Object
        );
    }

    [Fact]
    public async Task UsernameExistsAsync_ShouldReturnTrue_WhenUsernameExists()
    {
        // Arrange
        var username = "existinguser";
        _dbContext.Users.Add(new User
        {
            Username = username,
            CircleUserId = "circle123",
            CredentialId = "cred123",
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _authService.UsernameExistsAsync(username);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UsernameExistsAsync_ShouldReturnFalse_WhenUsernameDoesNotExist()
    {
        // Arrange
        var username = "newuser";

        // Act
        var result = await _authService.UsernameExistsAsync(username);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task InitiateRegistrationAsync_ShouldReturnChallenge_ForNewUser()
    {
        // Arrange
        var username = "newuser";
        var circleChallenge = new CircleRegistrationChallengeResponse
        {
            Challenge = "test-challenge-123",
            UserId = "circle-user-id",
            RpId = "circle.com",
            Timeout = 60000
        };

        _mockCircleService
            .Setup(x => x.InitiateUserRegistrationAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(circleChallenge);

        // Act
        var result = await _authService.InitiateRegistrationAsync(username);

        // Assert
        result.Should().NotBeNull();
        result.Challenge.Should().Be(circleChallenge.Challenge);
        result.UserId.Should().Be(circleChallenge.UserId);
        result.ChallengeId.Should().NotBeEmpty();
        _mockCircleService.Verify(x => x.InitiateUserRegistrationAsync(username, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitiateRegistrationAsync_ShouldThrowException_WhenUsernameExists()
    {
        // Arrange
        var username = "existinguser";
        _dbContext.Users.Add(new User
        {
            Username = username,
            CircleUserId = "circle123",
            CredentialId = "cred123",
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _authService.InitiateRegistrationAsync(username)
        );
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldCreateUser_WhenValidCredentials()
    {
        // Arrange
        var request = new CompleteRegistrationRequest
        {
            ChallengeId = "challenge123",
            Username = "newuser",
            CredentialId = "cred123",
            PublicKey = "pubkey123",
            AuthenticatorData = "authdata123"
        };

        var circleUser = new CircleUserResponse
        {
            UserId = "circle-user-id",
            Username = request.Username,
            CredentialId = request.CredentialId,
            CreatedAt = DateTime.UtcNow
        };

        _mockCircleService
            .Setup(x => x.CompleteUserRegistrationAsync(It.IsAny<CircleRegistrationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(circleUser);

        // Act
        var result = await _authService.CompleteRegistrationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(request.Username);
        result.CircleUserId.Should().Be(circleUser.UserId);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        user.Should().NotBeNull();
        user!.CircleUserId.Should().Be(circleUser.UserId);
        user.CredentialId.Should().Be(request.CredentialId);
    }

    [Fact]
    public async Task InitiateLoginAsync_ShouldReturnChallenge_ForExistingUser()
    {
        // Arrange
        var username = "existinguser";
        var circleUserId = "circle123";

        _dbContext.Users.Add(new User
        {
            Username = username,
            CircleUserId = circleUserId,
            CredentialId = "cred123",
            CreatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        var circleChallenge = new CircleAuthenticationChallengeResponse
        {
            Challenge = "login-challenge-123",
            RpId = "circle.com",
            Timeout = 60000,
            AllowedCredentials = new List<string> { "cred123" }
        };

        _mockCircleService
            .Setup(x => x.InitiateUserLoginAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(circleChallenge);

        // Act
        var result = await _authService.InitiateLoginAsync(username);

        // Assert
        result.Should().NotBeNull();
        result.Challenge.Should().Be(circleChallenge.Challenge);
        result.ChallengeId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task InitiateLoginAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var username = "nonexistentuser";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _authService.InitiateLoginAsync(username)
        );
    }

    [Fact]
    public async Task CompleteLoginAsync_ShouldReturnToken_WhenValidCredentials()
    {
        // Arrange
        var username = "existinguser";
        var circleUserId = "circle123";
        var expectedToken = "jwt-token-123";
        var walletAddress = "0x123456789";

        var user = new User
        {
            Username = username,
            CircleUserId = circleUserId,
            CredentialId = "cred123",
            WalletAddress = walletAddress,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var request = new CompleteLoginRequest
        {
            ChallengeId = "challenge-id",
            Username = username,
            CredentialId = "cred123",
            AuthenticatorData = "auth-data",
            Signature = "signature"
        };

        var circleAuth = new CircleAuthenticationResponse
        {
            UserId = circleUserId,
            Username = username,
            SessionToken = "circle-session-token",
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            WalletAddress = walletAddress
        };

        _mockCircleService
            .Setup(x => x.CompleteUserLoginAsync(It.IsAny<CircleAuthenticationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(circleAuth);

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns(expectedToken);

        // Act
        var result = await _authService.CompleteLoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(expectedToken);
        result.Username.Should().Be(username);
        result.WalletAddress.Should().Be(walletAddress);

        // Verify user's last login was updated
        var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        updatedUser.Should().NotBeNull();
        updatedUser!.LastLoginAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CompleteLoginAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new CompleteLoginRequest
        {
            ChallengeId = "challenge-id",
            Username = "nonexistentuser",
            CredentialId = "cred123",
            AuthenticatorData = "auth-data",
            Signature = "signature"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _authService.CompleteLoginAsync(request)
        );
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
