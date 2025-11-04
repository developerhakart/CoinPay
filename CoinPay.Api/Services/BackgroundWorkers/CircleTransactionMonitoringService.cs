using CoinPay.Api.Data;
using CoinPay.Api.Models;
using CoinPay.Api.Services.Circle;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Services.BackgroundWorkers;

/// <summary>
/// Background service that monitors pending Circle API transactions and updates their status
/// </summary>
public class CircleTransactionMonitoringService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CircleTransactionMonitoringService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _maxTransactionAge = TimeSpan.FromHours(24);

    public CircleTransactionMonitoringService(
        IServiceProvider serviceProvider,
        ILogger<CircleTransactionMonitoringService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Circle Transaction Monitoring Service started. Polling interval: {Interval}s",
            _pollingInterval.TotalSeconds);

        // Wait a bit before starting to allow the application to fully initialize
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MonitorPendingCircleTransactionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while monitoring Circle transactions");
            }

            try
            {
                await Task.Delay(_pollingInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
        }

        _logger.LogInformation("Circle Transaction Monitoring Service stopped");
    }

    private async Task MonitorPendingCircleTransactionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var circleService = scope.ServiceProvider.GetRequiredService<ICircleService>();
        var walletRepository = scope.ServiceProvider.GetRequiredService<CoinPay.Api.Repositories.IWalletRepository>();

        // Get all pending Circle transactions (POL transfers)
        // Note: Also includes transactions with empty/null TransactionId to mark them as failed
        var pendingTransactions = await db.Transactions
            .Where(t => t.Status == "Pending" &&
                       t.Currency == "POL")
            .ToListAsync(cancellationToken);

        if (pendingTransactions.Count == 0)
        {
            _logger.LogDebug("No pending Circle transactions to monitor");
            return;
        }

        _logger.LogInformation("Monitoring {Count} pending Circle transactions", pendingTransactions.Count);

        int updatedCount = 0;
        int failedCount = 0;

        foreach (var transaction in pendingTransactions)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            // Skip very old transactions (likely stuck)
            if ((DateTime.UtcNow - transaction.CreatedAt) > _maxTransactionAge)
            {
                _logger.LogWarning("Circle transaction {Id} is older than {Hours} hours, marking as failed",
                    transaction.Id, _maxTransactionAge.TotalHours);

                transaction.Status = "Failed";
                transaction.CompletedAt = DateTime.UtcNow;
                failedCount++;
                continue;
            }

            // Check if transaction has empty/null Circle transaction ID (API call failed)
            if (string.IsNullOrEmpty(transaction.TransactionId))
            {
                _logger.LogWarning("Circle transaction {Id} has empty TransactionId, marking as failed",
                    transaction.Id);

                transaction.Status = "Failed";
                transaction.CompletedAt = DateTime.UtcNow;
                failedCount++;
                continue;
            }

            try
            {
                // For developer-controlled wallets, we need to query all wallet transactions
                // TODO: Store wallet ID with transaction to avoid hardcoding user ID
                var userId = 1; // Hardcoded for now
                var wallet = await walletRepository.GetByUserIdAsync(userId);

                if (wallet == null || string.IsNullOrEmpty(wallet.CircleWalletId))
                {
                    _logger.LogWarning("No Circle wallet found for transaction {Id}", transaction.Id);
                    continue;
                }

                // Get all transactions for this wallet
                var walletTransactions = await circleService.GetWalletTransactionsAsync(
                    wallet.CircleWalletId,
                    cancellationToken);

                // Find our transaction by ID
                var circleStatus = walletTransactions.FirstOrDefault(t => t.TransactionId == transaction.TransactionId);

                if (circleStatus == null)
                {
                    _logger.LogDebug("Transaction {CircleTransactionId} not found in wallet transactions (may not be synced yet)",
                        transaction.TransactionId);
                    continue;
                }

                _logger.LogDebug("Circle transaction {Id} status from API: {Status}",
                    transaction.Id, circleStatus.Status);

                // Update transaction status based on Circle response
                var previousStatus = transaction.Status;
                transaction.Status = circleStatus.Status?.ToUpper() switch
                {
                    "CONFIRMED" => "Completed",
                    "COMPLETE" => "Completed",
                    "FAILED" => "Failed",
                    "CANCELLED" => "Failed",
                    _ => "Pending"
                };

                // Set completion timestamp if status changed to completed or failed
                if (transaction.Status != "Pending" && previousStatus == "Pending")
                {
                    transaction.CompletedAt = DateTime.UtcNow;
                    updatedCount++;

                    _logger.LogInformation(
                        "Circle transaction {Id} status updated: {OldStatus} → {NewStatus}, CircleTransactionId: {CircleTransactionId}",
                        transaction.Id,
                        previousStatus,
                        transaction.Status,
                        transaction.TransactionId);
                }
            }
            catch (Exception ex)
            {
                failedCount++;
                _logger.LogWarning(ex, "Failed to check Circle API status for transaction {Id} (CircleTransactionId: {CircleTransactionId})",
                    transaction.Id, transaction.TransactionId);
            }
        }

        // Save all changes
        if (updatedCount > 0 || failedCount > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
                "Circle monitoring cycle complete: {Updated} transactions updated, {Failed} checks failed",
                updatedCount, failedCount);
        }
    }
}
