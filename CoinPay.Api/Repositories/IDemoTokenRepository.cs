using CoinPay.Api.Models;

namespace CoinPay.Api.Repositories;

/// <summary>
/// Repository interface for managing demo token balances
/// </summary>
public interface IDemoTokenRepository
{
    /// <summary>
    /// Get demo token balance for a user and token symbol
    /// </summary>
    Task<DemoTokenBalance?> GetByUserAndTokenAsync(int userId, string tokenSymbol);

    /// <summary>
    /// Get all demo token balances for a user
    /// </summary>
    Task<List<DemoTokenBalance>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Create or update demo token balance
    /// </summary>
    Task<DemoTokenBalance> UpsertAsync(DemoTokenBalance balance);

    /// <summary>
    /// Issue demo tokens to a user
    /// </summary>
    Task<DemoTokenBalance> IssueDemoTokensAsync(int userId, string tokenSymbol, decimal amount);

    /// <summary>
    /// Deduct demo tokens from user balance (for investments)
    /// </summary>
    Task<bool> DeductBalanceAsync(int userId, string tokenSymbol, decimal amount);

    /// <summary>
    /// Add demo tokens to user balance (from investment returns)
    /// </summary>
    Task<bool> AddBalanceAsync(int userId, string tokenSymbol, decimal amount);

    /// <summary>
    /// Check if user has sufficient demo token balance
    /// </summary>
    Task<bool> HasSufficientBalanceAsync(int userId, string tokenSymbol, decimal amount);
}
