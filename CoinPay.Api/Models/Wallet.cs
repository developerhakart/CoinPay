namespace CoinPay.Api.Models;

/// <summary>
/// Represents a user's wallet in the CoinPay system
/// </summary>
public class Wallet
{
    /// <summary>
    /// Internal database ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User ID who owns this wallet
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Blockchain wallet address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Circle wallet ID
    /// </summary>
    public string CircleWalletId { get; set; } = string.Empty;

    /// <summary>
    /// Blockchain network (e.g., "MATIC-AMOY")
    /// </summary>
    public string Blockchain { get; set; } = string.Empty;

    /// <summary>
    /// Wallet type (e.g., "SCA" for Smart Contract Account)
    /// </summary>
    public string WalletType { get; set; } = string.Empty;

    /// <summary>
    /// Current USDC balance (cached)
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Balance currency (e.g., "USDC")
    /// </summary>
    public string BalanceCurrency { get; set; } = "USDC";

    /// <summary>
    /// Last time balance was updated
    /// </summary>
    public DateTime? BalanceUpdatedAt { get; set; }

    /// <summary>
    /// Wallet creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Navigation property to User
    /// </summary>
    public User? User { get; set; }
}
