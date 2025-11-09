using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Investment;

/// <summary>
/// Service for managing WhiteBIT demo tokens (DUSDT, DBTC)
/// </summary>
public interface IDemoTokenService
{
    /// <summary>
    /// Get all demo token balances for a user
    /// </summary>
    Task<List<DemoTokenBalance>> GetUserBalancesAsync(int userId);

    /// <summary>
    /// Get specific demo token balance for a user
    /// </summary>
    Task<DemoTokenBalance?> GetBalanceAsync(int userId, string tokenSymbol);

    /// <summary>
    /// Issue demo tokens to a user (initial allocation or top-up)
    /// </summary>
    Task<DemoTokenBalance> IssueTokensAsync(int userId, string tokenSymbol, decimal amount, string? notes = null);

    /// <summary>
    /// Check if user has sufficient balance for an investment
    /// </summary>
    Task<bool> HasSufficientBalanceAsync(int userId, string tokenSymbol, decimal amount);

    /// <summary>
    /// Deduct tokens from user balance (when creating investment)
    /// </summary>
    Task<bool> DeductBalanceAsync(int userId, string tokenSymbol, decimal amount);

    /// <summary>
    /// Add tokens to user balance (when closing investment with returns)
    /// </summary>
    Task<bool> AddBalanceAsync(int userId, string tokenSymbol, decimal amount);

    /// <summary>
    /// Validate if a token symbol is a supported demo token
    /// </summary>
    bool IsDemoToken(string tokenSymbol);

    /// <summary>
    /// Get list of supported demo token symbols
    /// </summary>
    List<string> GetSupportedDemoTokens();
}
