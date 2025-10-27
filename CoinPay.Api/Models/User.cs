namespace CoinPay.Api.Models;

/// <summary>
/// Represents a user in the CoinPay system
/// </summary>
public class User
{
    /// <summary>
    /// Internal database ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Circle user ID from Circle Web3 Services
    /// </summary>
    public string? CircleUserId { get; set; }

    /// <summary>
    /// WebAuthn credential ID for passkey authentication
    /// </summary>
    public string? CredentialId { get; set; }

    /// <summary>
    /// User creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// User's wallet address (if created)
    /// </summary>
    public string? WalletAddress { get; set; }
}
