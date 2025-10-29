namespace CoinPay.Api.Models;

/// <summary>
/// Represents a user's bank account for fiat payouts.
/// Sensitive data (routing number, account number) should be encrypted at rest.
/// </summary>
public class BankAccount
{
    /// <summary>
    /// Unique identifier for the bank account
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User ID who owns this bank account
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Name of the account holder
    /// </summary>
    public string AccountHolderName { get; set; } = string.Empty;

    /// <summary>
    /// Encrypted routing number (9 digits for US banks)
    /// </summary>
    public byte[] RoutingNumberEncrypted { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Encrypted account number (5-17 digits)
    /// </summary>
    public byte[] AccountNumberEncrypted { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Last 4 digits of account number (stored unencrypted for display)
    /// </summary>
    public string LastFourDigits { get; set; } = string.Empty;

    /// <summary>
    /// Type of account: "checking" or "savings"
    /// </summary>
    public string AccountType { get; set; } = string.Empty;

    /// <summary>
    /// Name of the bank (optional)
    /// </summary>
    public string? BankName { get; set; }

    /// <summary>
    /// Whether this is the user's primary bank account
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Whether the bank account has been verified
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// When the bank account was added
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the bank account was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual ICollection<PayoutTransaction> PayoutTransactions { get; set; } = new List<PayoutTransaction>();
}
