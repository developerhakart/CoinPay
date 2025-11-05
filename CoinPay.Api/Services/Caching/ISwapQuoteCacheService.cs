using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Caching;

/// <summary>
/// Service for caching swap quotes to reduce DEX API calls
/// </summary>
public interface ISwapQuoteCacheService
{
    /// <summary>
    /// Gets a cached swap quote if available
    /// </summary>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <param name="amount">Swap amount</param>
    /// <param name="slippage">Slippage tolerance</param>
    /// <returns>Cached quote or null if not found/expired</returns>
    Task<SwapQuoteResult?> GetCachedQuoteAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippage);

    /// <summary>
    /// Caches a swap quote with TTL
    /// </summary>
    /// <param name="quote">Quote to cache</param>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <param name="amount">Swap amount</param>
    /// <param name="slippage">Slippage tolerance</param>
    Task SetCachedQuoteAsync(
        SwapQuoteResult quote,
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippage);

    /// <summary>
    /// Invalidates cached quote for a token pair
    /// </summary>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    Task InvalidateCacheAsync(string fromToken, string toToken);
}
