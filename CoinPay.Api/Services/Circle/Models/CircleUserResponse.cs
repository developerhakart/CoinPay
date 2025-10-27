namespace CoinPay.Api.Services.Circle.Models;

/// <summary>
/// Response containing Circle user details.
/// </summary>
public class CircleUserResponse
{
    /// <summary>
    /// The unique Circle user identifier.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The username associated with the Circle user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The user's credential ID for WebAuthn.
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// The user's wallet ID, if a wallet has been created.
    /// </summary>
    public string? WalletId { get; set; }

    /// <summary>
    /// The user's wallet address, if a wallet has been created.
    /// </summary>
    public string? WalletAddress { get; set; }

    /// <summary>
    /// The timestamp when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
