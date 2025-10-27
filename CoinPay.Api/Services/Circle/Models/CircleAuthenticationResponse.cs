namespace CoinPay.Api.Services.Circle.Models;

/// <summary>
/// Response after successful user authentication with Circle.
/// </summary>
public class CircleAuthenticationResponse
{
    /// <summary>
    /// The unique Circle user identifier.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The username of the authenticated user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The session token for the authenticated session.
    /// </summary>
    public string SessionToken { get; set; } = string.Empty;

    /// <summary>
    /// The expiration time for the session token.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// The user's wallet ID, if available.
    /// </summary>
    public string? WalletId { get; set; }

    /// <summary>
    /// The user's wallet address, if available.
    /// </summary>
    public string? WalletAddress { get; set; }
}
