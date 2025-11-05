using CoinPay.Api.Models;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service for calculating swap transaction fees
/// </summary>
public interface IFeeCalculationService
{
    /// <summary>
    /// Calculates the platform fee for a swap
    /// </summary>
    /// <param name="tokenAddress">Token address for the fee</param>
    /// <param name="amount">Swap amount</param>
    /// <returns>Fee amount in the same token</returns>
    Task<decimal> CalculateSwapFeeAsync(string tokenAddress, decimal amount);

    /// <summary>
    /// Gets detailed fee breakdown for a swap
    /// </summary>
    /// <param name="fromToken">Source token address</param>
    /// <param name="toToken">Destination token address</param>
    /// <param name="fromAmount">Amount to swap</param>
    /// <returns>Detailed fee breakdown</returns>
    Task<FeeBreakdown> GetFeeBreakdownAsync(string fromToken, string toToken, decimal fromAmount);

    /// <summary>
    /// Gets the fee percentage for a user (may vary based on volume)
    /// </summary>
    /// <param name="userId">User ID (optional, for volume-based discounts)</param>
    /// <returns>Fee percentage (e.g., 0.5 for 0.5%)</returns>
    Task<decimal> GetFeePercentageAsync(Guid? userId = null);
}
