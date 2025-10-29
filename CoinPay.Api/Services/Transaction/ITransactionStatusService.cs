using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Transaction;

/// <summary>
/// Service for managing transaction status updates with cache invalidation and events
/// </summary>
public interface ITransactionStatusService
{
    /// <summary>
    /// Update transaction status with cache invalidation
    /// </summary>
    Task UpdateStatusAsync(int transactionId, TransactionStatus status, string? txHash = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update transaction with receipt and invalidate caches
    /// </summary>
    Task UpdateWithReceiptAsync(int transactionId, string txHash, long blockNumber, decimal gasUsed, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark transaction as failed
    /// </summary>
    Task MarkAsFailedAsync(int transactionId, string errorMessage, CancellationToken cancellationToken = default);
}
