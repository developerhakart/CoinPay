namespace CoinPay.Api.Services.Payout;

/// <summary>
/// Implementation of payout audit service
/// Provides comprehensive audit trail for compliance and troubleshooting
/// </summary>
public class PayoutAuditService : IPayoutAuditService
{
    private readonly ILogger<PayoutAuditService> _logger;

    // In a real implementation, this would write to a separate audit database table
    // For MVP, we're using structured logging which can be indexed by log aggregators
    private static readonly List<PayoutAuditEntry> _auditTrail = new();
    private static readonly object _lock = new();

    public PayoutAuditService(ILogger<PayoutAuditService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Log payout initiation
    /// </summary>
    public Task LogPayoutInitiatedAsync(Guid payoutId, int userId, decimal usdcAmount, Guid bankAccountId)
    {
        var entry = new PayoutAuditEntry
        {
            Id = Guid.NewGuid(),
            PayoutId = payoutId,
            EventType = "PAYOUT_INITIATED",
            Description = $"Payout initiated by user {userId}",
            NewValue = $"{usdcAmount} USDC",
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            Metadata = System.Text.Json.JsonSerializer.Serialize(new
            {
                usdcAmount,
                bankAccountId
            })
        };

        AddAuditEntry(entry);

        _logger.LogInformation(
            "AUDIT: Payout {PayoutId} initiated by user {UserId}, Amount: {Amount} USDC, BankAccount: {BankAccountId}",
            payoutId, userId, usdcAmount, bankAccountId);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Log payout status change
    /// </summary>
    public Task LogStatusChangeAsync(Guid payoutId, string previousStatus, string newStatus, string? reason = null)
    {
        var entry = new PayoutAuditEntry
        {
            Id = Guid.NewGuid(),
            PayoutId = payoutId,
            EventType = "STATUS_CHANGED",
            Description = $"Status changed from {previousStatus} to {newStatus}",
            PreviousValue = previousStatus,
            NewValue = newStatus,
            Timestamp = DateTime.UtcNow,
            Metadata = reason != null ? System.Text.Json.JsonSerializer.Serialize(new { reason }) : null
        };

        AddAuditEntry(entry);

        _logger.LogInformation(
            "AUDIT: Payout {PayoutId} status changed from {PreviousStatus} to {NewStatus}. Reason: {Reason}",
            payoutId, previousStatus, newStatus, reason ?? "N/A");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Log payout completion
    /// </summary>
    public Task LogPayoutCompletedAsync(Guid payoutId, decimal netAmount)
    {
        var entry = new PayoutAuditEntry
        {
            Id = Guid.NewGuid(),
            PayoutId = payoutId,
            EventType = "PAYOUT_COMPLETED",
            Description = "Payout completed successfully",
            NewValue = $"${netAmount}",
            Timestamp = DateTime.UtcNow,
            Metadata = System.Text.Json.JsonSerializer.Serialize(new { netAmount })
        };

        AddAuditEntry(entry);

        _logger.LogInformation(
            "AUDIT: Payout {PayoutId} completed successfully. Net amount: ${NetAmount}",
            payoutId, netAmount);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Log payout failure
    /// </summary>
    public Task LogPayoutFailedAsync(Guid payoutId, string failureReason)
    {
        var entry = new PayoutAuditEntry
        {
            Id = Guid.NewGuid(),
            PayoutId = payoutId,
            EventType = "PAYOUT_FAILED",
            Description = "Payout failed",
            NewValue = failureReason,
            Timestamp = DateTime.UtcNow,
            Metadata = System.Text.Json.JsonSerializer.Serialize(new { failureReason })
        };

        AddAuditEntry(entry);

        _logger.LogWarning(
            "AUDIT: Payout {PayoutId} failed. Reason: {FailureReason}",
            payoutId, failureReason);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Log payout cancellation
    /// </summary>
    public Task LogPayoutCancelledAsync(Guid payoutId, int userId)
    {
        var entry = new PayoutAuditEntry
        {
            Id = Guid.NewGuid(),
            PayoutId = payoutId,
            EventType = "PAYOUT_CANCELLED",
            Description = $"Payout cancelled by user {userId}",
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };

        AddAuditEntry(entry);

        _logger.LogInformation(
            "AUDIT: Payout {PayoutId} cancelled by user {UserId}",
            payoutId, userId);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Log webhook received
    /// </summary>
    public Task LogWebhookReceivedAsync(string gatewayTransactionId, string status, string payload)
    {
        // Note: We don't have payoutId here, so we log with gateway transaction ID
        _logger.LogInformation(
            "AUDIT: Webhook received for gateway transaction {GatewayTxId}, Status: {Status}",
            gatewayTransactionId, status);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Get audit trail for payout
    /// </summary>
    public Task<List<PayoutAuditEntry>> GetAuditTrailAsync(Guid payoutId)
    {
        lock (_lock)
        {
            var entries = _auditTrail
                .Where(e => e.PayoutId == payoutId)
                .OrderBy(e => e.Timestamp)
                .ToList();

            return Task.FromResult(entries);
        }
    }

    /// <summary>
    /// Add audit entry to in-memory store
    /// </summary>
    private void AddAuditEntry(PayoutAuditEntry entry)
    {
        lock (_lock)
        {
            _auditTrail.Add(entry);

            // Keep only last 10,000 entries in memory (for MVP)
            // In production, this would be persisted to database
            if (_auditTrail.Count > 10000)
            {
                _auditTrail.RemoveRange(0, 1000);
            }
        }
    }
}
