namespace CoinPay.Api.Services.Payout;

/// <summary>
/// Service for auditing payout operations
/// Provides compliance logging and audit trail functionality
/// </summary>
public interface IPayoutAuditService
{
    /// <summary>
    /// Log payout initiation
    /// </summary>
    Task LogPayoutInitiatedAsync(Guid payoutId, int userId, decimal usdcAmount, Guid bankAccountId);

    /// <summary>
    /// Log payout status change
    /// </summary>
    Task LogStatusChangeAsync(Guid payoutId, string previousStatus, string newStatus, string? reason = null);

    /// <summary>
    /// Log payout completion
    /// </summary>
    Task LogPayoutCompletedAsync(Guid payoutId, decimal netAmount);

    /// <summary>
    /// Log payout failure
    /// </summary>
    Task LogPayoutFailedAsync(Guid payoutId, string failureReason);

    /// <summary>
    /// Log payout cancellation
    /// </summary>
    Task LogPayoutCancelledAsync(Guid payoutId, int userId);

    /// <summary>
    /// Log webhook received
    /// </summary>
    Task LogWebhookReceivedAsync(string gatewayTransactionId, string status, string payload);

    /// <summary>
    /// Get audit trail for payout
    /// </summary>
    Task<List<PayoutAuditEntry>> GetAuditTrailAsync(Guid payoutId);
}

/// <summary>
/// Audit trail entry
/// </summary>
public class PayoutAuditEntry
{
    public Guid Id { get; set; }
    public Guid PayoutId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public int? UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Metadata { get; set; }
}
