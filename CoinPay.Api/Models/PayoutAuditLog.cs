namespace CoinPay.Api.Models;

/// <summary>
/// Audit log for payout transaction events
/// </summary>
public class PayoutAuditLog
{
    /// <summary>
    /// Unique identifier for the audit log entry
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Payout transaction ID this log belongs to
    /// </summary>
    public Guid PayoutTransactionId { get; set; }

    /// <summary>
    /// Type of event (e.g., "initiated", "status_changed", "webhook_received", "cancelled", "completed", "failed")
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Previous status (for status_changed events)
    /// </summary>
    public string? PreviousStatus { get; set; }

    /// <summary>
    /// New status (for status_changed events)
    /// </summary>
    public string? NewStatus { get; set; }

    /// <summary>
    /// Additional event data in JSON format
    /// </summary>
    public string? EventData { get; set; }

    /// <summary>
    /// When the event occurred
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual PayoutTransaction? PayoutTransaction { get; set; }
}
