namespace CoinPay.Api.Models;

/// <summary>
/// Represents a crypto-to-fiat payout transaction
/// </summary>
public class PayoutTransaction
{
    /// <summary>
    /// Unique identifier for the payout transaction
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User ID who initiated the payout
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Bank account ID where funds will be sent
    /// </summary>
    public Guid BankAccountId { get; set; }

    /// <summary>
    /// External gateway transaction ID
    /// </summary>
    public string? GatewayTransactionId { get; set; }

    /// <summary>
    /// Amount in USDC being converted
    /// </summary>
    public decimal UsdcAmount { get; set; }

    /// <summary>
    /// Amount in USD after conversion (before fees)
    /// </summary>
    public decimal UsdAmount { get; set; }

    /// <summary>
    /// Exchange rate used (USDC to USD)
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Conversion fee in USD
    /// </summary>
    public decimal ConversionFee { get; set; }

    /// <summary>
    /// Payout/transfer fee in USD
    /// </summary>
    public decimal PayoutFee { get; set; }

    /// <summary>
    /// Total fees (conversion + payout)
    /// </summary>
    public decimal TotalFees { get; set; }

    /// <summary>
    /// Net amount user receives in USD (after all fees)
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Current status of the payout
    /// Values: pending, processing, completed, failed, cancelled
    /// </summary>
    public string Status { get; set; } = "pending";

    /// <summary>
    /// Reason for failure (if status is failed)
    /// </summary>
    public string? FailureReason { get; set; }

    /// <summary>
    /// When the payout was initiated
    /// </summary>
    public DateTime InitiatedAt { get; set; }

    /// <summary>
    /// When the payout was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Estimated arrival date for the funds
    /// </summary>
    public DateTime? EstimatedArrival { get; set; }

    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual BankAccount? BankAccount { get; set; }
    public virtual ICollection<PayoutAuditLog> AuditLogs { get; set; } = new List<PayoutAuditLog>();
}
