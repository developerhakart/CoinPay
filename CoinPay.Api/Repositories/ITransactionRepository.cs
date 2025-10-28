using CoinPay.Api.Models;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository interface for blockchain transaction operations
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Create a new blockchain transaction record
    /// </summary>
    Task<BlockchainTransaction> CreateAsync(BlockchainTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transaction by internal ID
    /// </summary>
    Task<BlockchainTransaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transaction by UserOperation hash
    /// </summary>
    Task<BlockchainTransaction?> GetByUserOpHashAsync(string userOpHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transaction by on-chain transaction hash
    /// </summary>
    Task<BlockchainTransaction?> GetByTransactionHashAsync(string txHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all transactions for a specific wallet
    /// </summary>
    Task<List<BlockchainTransaction>> GetByWalletIdAsync(int walletId, int limit = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get transactions by wallet address
    /// </summary>
    Task<List<BlockchainTransaction>> GetByWalletAddressAsync(string address, int limit = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update transaction status
    /// </summary>
    Task UpdateStatusAsync(int transactionId, TransactionStatus status, string? txHash = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update transaction with receipt information
    /// </summary>
    Task UpdateWithReceiptAsync(int transactionId, string txHash, long blockNumber, decimal gasUsed, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if UserOperation hash exists
    /// </summary>
    Task<bool> ExistsAsync(string userOpHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending transactions for a wallet
    /// </summary>
    Task<List<BlockchainTransaction>> GetPendingByWalletIdAsync(int walletId, CancellationToken cancellationToken = default);
}
