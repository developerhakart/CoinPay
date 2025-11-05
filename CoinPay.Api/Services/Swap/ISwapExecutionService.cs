using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service for executing token swaps
/// </summary>
public interface ISwapExecutionService
{
    /// <summary>
    /// Executes a token swap
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="walletAddress">Wallet address performing swap</param>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <param name="fromAmount">Amount to swap</param>
    /// <param name="slippageTolerance">Slippage tolerance percentage</param>
    /// <returns>Swap execution result with transaction hash</returns>
    Task<SwapExecutionResult> ExecuteSwapAsync(
        Guid userId,
        string walletAddress,
        string fromToken,
        string toToken,
        decimal fromAmount,
        decimal slippageTolerance);
}
