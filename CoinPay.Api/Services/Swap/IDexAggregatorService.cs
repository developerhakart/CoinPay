using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Interface for DEX aggregator services that provide swap quotes and transaction data
/// </summary>
public interface IDexAggregatorService
{
    /// <summary>
    /// Gets a swap quote for token exchange
    /// </summary>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <param name="amount">Amount to swap</param>
    /// <param name="slippageTolerance">Slippage tolerance percentage (0.5, 1, 3, etc.)</param>
    /// <returns>Swap quote with exchange rate and estimated amounts</returns>
    Task<SwapQuote> GetQuoteAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippageTolerance);

    /// <summary>
    /// Gets swap transaction data ready to be signed and submitted
    /// </summary>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <param name="amount">Amount to swap</param>
    /// <param name="slippageTolerance">Slippage tolerance percentage</param>
    /// <param name="fromAddress">Sender wallet address</param>
    /// <returns>Transaction data ready for execution</returns>
    Task<DexSwapTransaction> GetSwapTransactionAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippageTolerance,
        string fromAddress);

    /// <summary>
    /// Estimates gas cost for a swap transaction
    /// </summary>
    /// <param name="swapTx">Swap transaction data</param>
    /// <returns>Estimated gas cost in native token</returns>
    Task<decimal> EstimateGasAsync(DexSwapTransaction swapTx);
}
