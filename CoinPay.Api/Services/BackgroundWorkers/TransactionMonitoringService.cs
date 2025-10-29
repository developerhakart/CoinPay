using CoinPay.Api.Repositories;
using CoinPay.Api.Services.UserOperation;
using CoinPay.Api.Services.Caching;
using CoinPay.Api.Models;

namespace CoinPay.Api.Services.BackgroundWorkers;

/// <summary>
/// Background service that monitors pending transactions and updates their status
/// </summary>
public class TransactionMonitoringService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TransactionMonitoringService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _maxTransactionAge = TimeSpan.FromHours(24);

    public TransactionMonitoringService(
        IServiceProvider serviceProvider,
        ILogger<TransactionMonitoringService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Transaction Monitoring Service started. Polling interval: {Interval}s",
            _pollingInterval.TotalSeconds);

        // Wait a bit before starting to allow the application to fully initialize
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MonitorPendingTransactionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while monitoring transactions");
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

        _logger.LogInformation("Transaction Monitoring Service stopped");
    }

    private async Task MonitorPendingTransactionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var userOpService = scope.ServiceProvider.GetRequiredService<IUserOperationService>();
        var cachingService = scope.ServiceProvider.GetService<ICachingService>();

        // Get all wallets with pending transactions
        var allWallets = await GetAllWalletIdsWithPendingTransactionsAsync(transactionRepository, cancellationToken);

        if (allWallets.Count == 0)
        {
            _logger.LogDebug("No pending transactions to monitor");
            return;
        }

        _logger.LogInformation("Monitoring {Count} wallets with pending transactions", allWallets.Count);

        int updatedCount = 0;
        int failedCount = 0;

        foreach (var walletId in allWallets)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var pendingTransactions = await transactionRepository.GetPendingByWalletIdAsync(walletId, cancellationToken);

            foreach (var transaction in pendingTransactions)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // Skip very old transactions (likely stuck)
                if ((DateTime.UtcNow - transaction.CreatedAt) > _maxTransactionAge)
                {
                    _logger.LogWarning("Transaction {Id} is older than {Hours} hours, skipping",
                        transaction.Id, _maxTransactionAge.TotalHours);
                    continue;
                }

                try
                {
                    var receipt = await userOpService.GetReceiptAsync(transaction.UserOpHash, cancellationToken);

                    if (receipt != null)
                    {
                        // Transaction confirmed!
                        await transactionRepository.UpdateWithReceiptAsync(
                            transaction.Id,
                            receipt.TransactionHash,
                            receipt.BlockNumber,
                            receipt.ActualGasUsed,
                            cancellationToken);

                        // Invalidate balance cache for affected wallets
                        if (cachingService != null)
                        {
                            var cacheKeyFrom = $"wallet:balance:{transaction.FromAddress}";
                            var cacheKeyTo = $"wallet:balance:{transaction.ToAddress}";
                            await cachingService.RemoveAsync(cacheKeyFrom);
                            await cachingService.RemoveAsync(cacheKeyTo);
                        }

                        updatedCount++;

                        _logger.LogInformation(
                            "Transaction {Id} confirmed: TxHash={TxHash}, Block={BlockNumber}",
                            transaction.Id, receipt.TransactionHash, receipt.BlockNumber);
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    _logger.LogWarning(ex, "Failed to check receipt for transaction {Id}", transaction.Id);
                }
            }
        }

        if (updatedCount > 0 || failedCount > 0)
        {
            _logger.LogInformation(
                "Monitoring cycle complete: {Updated} transactions updated, {Failed} failed",
                updatedCount, failedCount);
        }
    }

    private async Task<List<int>> GetAllWalletIdsWithPendingTransactionsAsync(
        ITransactionRepository repository,
        CancellationToken cancellationToken)
    {
        // This is a simplified approach - in production you might want a more efficient query
        var walletIds = new List<int>();

        // Get a sample of recent pending transactions to find active wallets
        // In production, you'd have a better way to track which wallets have pending transactions
        for (int walletId = 1; walletId <= 100; walletId++)
        {
            var pending = await repository.GetPendingByWalletIdAsync(walletId, cancellationToken);
            if (pending.Count > 0)
            {
                walletIds.Add(walletId);
            }

            if (walletIds.Count >= 50) // Limit to prevent too many concurrent checks
                break;
        }

        return walletIds;
    }
}
