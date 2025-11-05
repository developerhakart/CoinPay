using CoinPay.Api.Models;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository for swap transaction data access
/// </summary>
public interface ISwapTransactionRepository
{
    /// <summary>
    /// Creates a new swap transaction record
    /// </summary>
    /// <param name="transaction">Swap transaction to create</param>
    /// <returns>Created swap transaction with generated ID</returns>
    Task<SwapTransaction> CreateAsync(SwapTransaction transaction);

    /// <summary>
    /// Gets a swap transaction by ID
    /// </summary>
    /// <param name="id">Swap transaction ID</param>
    /// <returns>Swap transaction or null if not found</returns>
    Task<SwapTransaction?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets a swap transaction by blockchain transaction hash
    /// </summary>
    /// <param name="txHash">Transaction hash</param>
    /// <returns>Swap transaction or null if not found</returns>
    Task<SwapTransaction?> GetByTransactionHashAsync(string txHash);

    /// <summary>
    /// Gets swap transactions for a user with pagination
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="status">Filter by status (optional)</param>
    /// <returns>List of swap transactions</returns>
    Task<List<SwapTransaction>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int pageSize = 20,
        SwapStatus? status = null);

    /// <summary>
    /// Gets swap transactions for a wallet address with pagination
    /// </summary>
    /// <param name="walletAddress">Wallet address</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>List of swap transactions</returns>
    Task<List<SwapTransaction>> GetByWalletAddressAsync(
        string walletAddress,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Updates an existing swap transaction
    /// </summary>
    /// <param name="transaction">Swap transaction to update</param>
    Task UpdateAsync(SwapTransaction transaction);

    /// <summary>
    /// Gets total count of swaps for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="status">Filter by status (optional)</param>
    /// <returns>Total swap count</returns>
    Task<int> GetSwapCountByUserAsync(Guid userId, SwapStatus? status = null);

    /// <summary>
    /// Gets total swap volume for a user (in USD equivalent)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Total volume in USD</returns>
    Task<decimal> GetTotalVolumeByUserAsync(Guid userId);
}
