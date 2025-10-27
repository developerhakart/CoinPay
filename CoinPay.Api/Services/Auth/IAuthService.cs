namespace CoinPay.Api.Services.Auth;

/// <summary>
/// Authentication service interface for user registration and login with passkeys
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Initiates the user registration process with Circle SDK
    /// </summary>
    /// <param name="username">Desired username</param>
    /// <returns>Challenge data for WebAuthn credential creation</returns>
    Task<RegistrationChallengeResponse> InitiateRegistrationAsync(string username);

    /// <summary>
    /// Completes the user registration after passkey creation
    /// </summary>
    /// <param name="request">Registration completion request with WebAuthn response</param>
    /// <returns>Created user with Circle credentials</returns>
    Task<UserRegistrationResponse> CompleteRegistrationAsync(CompleteRegistrationRequest request);

    /// <summary>
    /// Initiates the login process with passkey challenge
    /// </summary>
    /// <param name="username">Username to authenticate</param>
    /// <returns>Challenge data for WebAuthn authentication</returns>
    Task<LoginChallengeResponse> InitiateLoginAsync(string username);

    /// <summary>
    /// Completes the login process after passkey verification
    /// </summary>
    /// <param name="request">Login completion request with WebAuthn response</param>
    /// <returns>JWT token and user information</returns>
    Task<LoginResponse> CompleteLoginAsync(CompleteLoginRequest request);

    /// <summary>
    /// Verifies if a username is already taken
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <returns>True if username exists, false otherwise</returns>
    Task<bool> UsernameExistsAsync(string username);
}

/// <summary>
/// Response for registration initiation
/// </summary>
public class RegistrationChallengeResponse
{
    public string ChallengeId { get; set; } = string.Empty;
    public string Challenge { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// Request for completing registration
/// </summary>
public class CompleteRegistrationRequest
{
    public string ChallengeId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string CredentialId { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string AuthenticatorData { get; set; } = string.Empty;
}

/// <summary>
/// Response for completed registration
/// </summary>
public class UserRegistrationResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string CircleUserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Response for login challenge
/// </summary>
public class LoginChallengeResponse
{
    public string ChallengeId { get; set; } = string.Empty;
    public string Challenge { get; set; } = string.Empty;
}

/// <summary>
/// Request for completing login
/// </summary>
public class CompleteLoginRequest
{
    public string ChallengeId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string CredentialId { get; set; } = string.Empty;
    public string AuthenticatorData { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
}

/// <summary>
/// Response for completed login
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? WalletAddress { get; set; }
    public DateTime ExpiresAt { get; set; }
}
