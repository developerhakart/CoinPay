using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service for calculating slippage tolerance and minimum received amounts
/// </summary>
public interface ISlippageToleranceService
{
    /// <summary>
    /// Calculates the minimum amount to receive based on slippage tolerance
    /// </summary>
    /// <param name="expectedAmount">Expected amount to receive</param>
    /// <param name="slippagePercentage">Slippage tolerance percentage (e.g., 1 for 1%)</param>
    /// <returns>Minimum amount that must be received</returns>
    decimal CalculateMinimumReceived(decimal expectedAmount, decimal slippagePercentage);

    /// <summary>
    /// Validates slippage percentage is within acceptable range
    /// </summary>
    /// <param name="slippagePercentage">Slippage percentage to validate</param>
    /// <exception cref="ArgumentException">Thrown if slippage is out of range</exception>
    void ValidateSlippage(decimal slippagePercentage);

    /// <summary>
    /// Checks if slippage tolerance is excessive (may indicate high price impact)
    /// </summary>
    /// <param name="slippagePercentage">Slippage percentage</param>
    /// <returns>True if slippage is excessive (>5%)</returns>
    bool IsSlippageExcessive(decimal slippagePercentage);

    /// <summary>
    /// Gets recommended slippage based on trade parameters
    /// </summary>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <param name="amount">Swap amount in USD equivalent</param>
    /// <returns>Slippage recommendation</returns>
    SlippageRecommendation GetRecommendedSlippage(string fromToken, string toToken, decimal amount);
}
