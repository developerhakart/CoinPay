using CoinPay.Api.Repositories;
using CoinPay.Api.Services.FiatGateway;

namespace CoinPay.Api.Services.Payout;

/// <summary>
/// Service for managing payout status updates with state machine logic
/// </summary>
public class PayoutStatusService : IPayoutStatusService
{
    private readonly IPayoutRepository _payoutRepository;
    private readonly IFiatGatewayService _fiatGatewayService;
    private readonly ILogger<PayoutStatusService> _logger;

    // Valid status transitions (state machine)
    private static readonly Dictionary<string, List<string>> ValidTransitions = new()
    {
        { "pending", new List<string> { "processing", "failed", "cancelled" } },
        { "processing", new List<string> { "completed", "failed" } },
        { "completed", new List<string>() }, // Terminal state
        { "failed", new List<string>() }, // Terminal state
        { "cancelled", new List<string>() } // Terminal state
    };

    public PayoutStatusService(
        IPayoutRepository payoutRepository,
        IFiatGatewayService fiatGatewayService,
        ILogger<PayoutStatusService> logger)
    {
        _payoutRepository = payoutRepository;
        _fiatGatewayService = fiatGatewayService;
        _logger = logger;
    }

    /// <summary>
    /// Update payout status with validation
    /// </summary>
    public async Task<(bool Success, string? ErrorMessage)> UpdateStatusAsync(
        Guid payoutId,
        string newStatus,
        string? failureReason = null)
    {
        try
        {
            var payout = await _payoutRepository.GetByIdAsync(payoutId);

            if (payout == null)
            {
                _logger.LogWarning("UpdateStatusAsync: Payout {PayoutId} not found", payoutId);
                return (false, "Payout not found");
            }

            // Validate status transition
            if (!IsValidStatusTransition(payout.Status, newStatus))
            {
                _logger.LogWarning("UpdateStatusAsync: Invalid status transition for payout {PayoutId}: {CurrentStatus} -> {NewStatus}",
                    payoutId, payout.Status, newStatus);
                return (false, $"Invalid status transition from {payout.Status} to {newStatus}");
            }

            var previousStatus = payout.Status;
            payout.Status = newStatus;

            // Update timestamps based on status
            if (newStatus == "completed" || newStatus == "failed" || newStatus == "cancelled")
            {
                if (!payout.CompletedAt.HasValue)
                {
                    payout.CompletedAt = DateTime.UtcNow;
                }
            }

            // Update failure reason if failed
            if (newStatus == "failed" && !string.IsNullOrEmpty(failureReason))
            {
                payout.FailureReason = failureReason;
            }

            await _payoutRepository.UpdateAsync(payout);

            _logger.LogInformation("UpdateStatusAsync: Updated payout {PayoutId} status from {PreviousStatus} to {NewStatus}",
                payoutId, previousStatus, newStatus);

            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateStatusAsync: Error updating payout {PayoutId} status to {NewStatus}",
                payoutId, newStatus);
            return (false, "Internal error updating status");
        }
    }

    /// <summary>
    /// Sync payout status with gateway
    /// </summary>
    public async Task<bool> SyncWithGatewayAsync(Guid payoutId)
    {
        try
        {
            var payout = await _payoutRepository.GetByIdAsync(payoutId);

            if (payout == null)
            {
                _logger.LogWarning("SyncWithGatewayAsync: Payout {PayoutId} not found", payoutId);
                return false;
            }

            // Only sync if payout is in progress
            if (payout.Status != "pending" && payout.Status != "processing")
            {
                _logger.LogDebug("SyncWithGatewayAsync: Payout {PayoutId} has terminal status {Status}, skipping sync",
                    payoutId, payout.Status);
                return true;
            }

            if (string.IsNullOrEmpty(payout.GatewayTransactionId))
            {
                _logger.LogWarning("SyncWithGatewayAsync: Payout {PayoutId} has no gateway transaction ID", payoutId);
                return false;
            }

            // Fetch status from gateway
            var gatewayStatus = await _fiatGatewayService.GetPayoutStatusAsync(payout.GatewayTransactionId);

            if (gatewayStatus == null)
            {
                _logger.LogWarning("SyncWithGatewayAsync: Failed to fetch status from gateway for payout {PayoutId}", payoutId);
                return false;
            }

            // Update if status changed
            if (gatewayStatus.Status != payout.Status)
            {
                var result = await UpdateStatusAsync(payoutId, gatewayStatus.Status, gatewayStatus.FailureReason);

                if (result.Success)
                {
                    // Update additional fields from gateway
                    if (gatewayStatus.CompletedAt.HasValue)
                    {
                        payout.CompletedAt = gatewayStatus.CompletedAt;
                        await _payoutRepository.UpdateAsync(payout);
                    }

                    _logger.LogInformation("SyncWithGatewayAsync: Synced payout {PayoutId} with gateway, new status: {Status}",
                        payoutId, gatewayStatus.Status);
                }
                else
                {
                    _logger.LogWarning("SyncWithGatewayAsync: Failed to update payout {PayoutId} status: {Error}",
                        payoutId, result.ErrorMessage);
                    return false;
                }
            }
            else
            {
                _logger.LogDebug("SyncWithGatewayAsync: Payout {PayoutId} status unchanged: {Status}",
                    payoutId, payout.Status);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SyncWithGatewayAsync: Error syncing payout {PayoutId} with gateway", payoutId);
            return false;
        }
    }

    /// <summary>
    /// Get payouts needing status updates
    /// </summary>
    public async Task<List<Guid>> GetPayoutsNeedingUpdateAsync(int limit = 100)
    {
        try
        {
            var pendingPayouts = await _payoutRepository.GetByStatusAsync("pending", limit);
            var processingPayouts = await _payoutRepository.GetByStatusAsync("processing", limit);

            var payoutsNeedingUpdate = pendingPayouts
                .Concat(processingPayouts)
                .OrderBy(p => p.InitiatedAt)
                .Take(limit)
                .Select(p => p.Id)
                .ToList();

            _logger.LogInformation("GetPayoutsNeedingUpdateAsync: Found {Count} payouts needing updates", payoutsNeedingUpdate.Count);

            return payoutsNeedingUpdate;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPayoutsNeedingUpdateAsync: Error fetching payouts needing updates");
            return new List<Guid>();
        }
    }

    /// <summary>
    /// Mark payout as completed
    /// </summary>
    public async Task<bool> MarkAsCompletedAsync(Guid payoutId, DateTime? completedAt = null)
    {
        var result = await UpdateStatusAsync(payoutId, "completed");

        if (result.Success && completedAt.HasValue)
        {
            var payout = await _payoutRepository.GetByIdAsync(payoutId);
            if (payout != null)
            {
                payout.CompletedAt = completedAt.Value;
                await _payoutRepository.UpdateAsync(payout);
            }
        }

        return result.Success;
    }

    /// <summary>
    /// Mark payout as failed
    /// </summary>
    public async Task<bool> MarkAsFailedAsync(Guid payoutId, string failureReason)
    {
        var result = await UpdateStatusAsync(payoutId, "failed", failureReason);
        return result.Success;
    }

    /// <summary>
    /// Validate status transition using state machine
    /// </summary>
    public bool IsValidStatusTransition(string currentStatus, string newStatus)
    {
        // Normalize status values
        currentStatus = currentStatus?.ToLowerInvariant() ?? "";
        newStatus = newStatus?.ToLowerInvariant() ?? "";

        // Same status is always valid (idempotent)
        if (currentStatus == newStatus)
        {
            return true;
        }

        // Check if transition exists in state machine
        if (!ValidTransitions.TryGetValue(currentStatus, out var allowedTransitions))
        {
            _logger.LogWarning("IsValidStatusTransition: Unknown current status: {CurrentStatus}", currentStatus);
            return false;
        }

        var isValid = allowedTransitions.Contains(newStatus);

        if (!isValid)
        {
            _logger.LogWarning("IsValidStatusTransition: Invalid transition from {CurrentStatus} to {NewStatus}",
                currentStatus, newStatus);
        }

        return isValid;
    }
}
