using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service for fetching and processing swap quotes
/// </summary>
public interface ISwapQuoteService
{
    /// <summary>
    /// Gets the best swap quote from available DEX aggregators
    /// </summary>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <param name="fromAmount">Amount to swap</param>
    /// <param name="slippageTolerance">Slippage tolerance percentage</param>
    /// <returns>Complete swap quote with fees and price impact</returns>
    Task<SwapQuoteResult> GetBestQuoteAsync(
        string fromToken,
        string toToken,
        decimal fromAmount,
        decimal slippageTolerance);

    /// <summary>
    /// Validates a token pair is supported for swapping
    /// </summary>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <returns>True if pair is valid</returns>
    /// <exception cref="InvalidOperationException">Thrown if pair is invalid</exception>
    Task<bool> ValidateTokenPairAsync(string fromToken, string toToken);
}
