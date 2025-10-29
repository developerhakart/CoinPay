using CoinPay.Api.Models;
using CoinPay.Api.Repositories;
using CoinPay.Api.Services.Caching;
using CoinPay.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CoinPay.Api.Services.Transaction;

/// <summary>
/// Service for managing transaction status updates with cache invalidation and events
/// </summary>
public class TransactionStatusService : ITransactionStatusService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICachingService? _cachingService;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<TransactionStatusService> _logger;

    public TransactionStatusService(
        ITransactionRepository transactionRepository,
        AppDbContext dbContext,
        ILogger<TransactionStatusService> logger,
        ICachingService? cachingService = null)
    {
        _transactionRepository = transactionRepository;
        _dbContext = dbContext;
        _cachingService = cachingService;
        _logger = logger;
    }

    public async Task UpdateStatusAsync(
        int transactionId,
        TransactionStatus status,
        string? txHash = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating transaction {TransactionId} status to {Status}", transactionId, status);

        var transaction = await _dbContext.BlockchainTransactions
            .Include(t => t.Wallet)
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction {TransactionId} not found", transactionId);
            return;
        }

        // Update status in repository
        await _transactionRepository.UpdateStatusAsync(transactionId, status, txHash, cancellationToken);

        // Invalidate balance cache for affected addresses
        await InvalidateBalanceCachesAsync(transaction.FromAddress, transaction.ToAddress);

        _logger.LogInformation("Transaction {TransactionId} status updated successfully", transactionId);
    }

    public async Task UpdateWithReceiptAsync(
        int transactionId,
        string txHash,
        long blockNumber,
        decimal gasUsed,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Updating transaction {TransactionId} with receipt: TxHash={TxHash}, Block={BlockNumber}",
            transactionId, txHash, blockNumber);

        var transaction = await _dbContext.BlockchainTransactions
            .Include(t => t.Wallet)
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction {TransactionId} not found", transactionId);
            return;
        }

        // Update with receipt in repository
        await _transactionRepository.UpdateWithReceiptAsync(transactionId, txHash, blockNumber, gasUsed, cancellationToken);

        // Invalidate balance cache for affected addresses
        await InvalidateBalanceCachesAsync(transaction.FromAddress, transaction.ToAddress);

        _logger.LogInformation("Transaction {TransactionId} updated with receipt successfully", transactionId);

        // TODO: Send webhook notification if configured
        // await SendWebhookNotificationAsync(transaction, txHash);
    }

    public async Task MarkAsFailedAsync(
        int transactionId,
        string errorMessage,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Marking transaction {TransactionId} as failed: {Error}", transactionId, errorMessage);

        var transaction = await _dbContext.BlockchainTransactions.FindAsync(
            new object[] { transactionId },
            cancellationToken);

        if (transaction == null)
        {
            _logger.LogWarning("Transaction {TransactionId} not found", transactionId);
            return;
        }

        transaction.Status = TransactionStatus.Failed;
        transaction.ErrorMessage = errorMessage;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Transaction {TransactionId} marked as failed", transactionId);

        // TODO: Send failure notification
    }

    private async Task InvalidateBalanceCachesAsync(string fromAddress, string toAddress)
    {
        if (_cachingService == null)
            return;

        try
        {
            var cacheKeyFrom = $"wallet:balance:{fromAddress}";
            var cacheKeyTo = $"wallet:balance:{toAddress}";

            await _cachingService.RemoveAsync(cacheKeyFrom);
            await _cachingService.RemoveAsync(cacheKeyTo);

            _logger.LogDebug("Invalidated balance caches for {From} and {To}", fromAddress, toAddress);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to invalidate balance caches");
        }
    }
}
