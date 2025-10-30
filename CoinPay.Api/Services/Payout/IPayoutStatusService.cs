namespace CoinPay.Api.Services.Payout;

/// <summary>
/// Service for managing payout status updates and state transitions
/// </summary>
public interface IPayoutStatusService
{
    /// <summary>
    /// Update payout status with validation and logging
    /// </summary>
    /// <param name="payoutId">Payout ID</param>
    /// <param name="newStatus">New status</param>
    /// <param name="failureReason">Optional failure reason</param>
    /// <returns>True if update successful</returns>
    Task<(bool Success, string? ErrorMessage)> UpdateStatusAsync(Guid payoutId, string newStatus, string? failureReason = null);

    /// <summary>
    /// Sync payout status with gateway
    /// </summary>
    /// <param name="payoutId">Payout ID</param>
    /// <returns>True if sync successful</returns>
    Task<bool> SyncWithGatewayAsync(Guid payoutId);

    /// <summary>
    /// Get all payouts that need status updates (pending/processing)
    /// </summary>
    /// <param name="limit">Maximum number to fetch</param>
    /// <returns>List of payout IDs needing updates</returns>
    Task<List<Guid>> GetPayoutsNeedingUpdateAsync(int limit = 100);

    /// <summary>
    /// Mark payout as completed
    /// </summary>
    /// <param name="payoutId">Payout ID</param>
    /// <param name="completedAt">Completion timestamp</param>
    /// <returns>True if successful</returns>
    Task<bool> MarkAsCompletedAsync(Guid payoutId, DateTime? completedAt = null);

    /// <summary>
    /// Mark payout as failed
    /// </summary>
    /// <param name="payoutId">Payout ID</param>
    /// <param name="failureReason">Reason for failure</param>
    /// <returns>True if successful</returns>
    Task<bool> MarkAsFailedAsync(Guid payoutId, string failureReason);

    /// <summary>
    /// Validate status transition
    /// </summary>
    /// <param name="currentStatus">Current status</param>
    /// <param name="newStatus">Proposed new status</param>
    /// <returns>True if transition is valid</returns>
    bool IsValidStatusTransition(string currentStatus, string newStatus);
}
