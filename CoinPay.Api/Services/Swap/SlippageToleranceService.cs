using CoinPay.Api.Models;
using Microsoft.Extensions.Logging;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service for handling slippage tolerance calculations
/// </summary>
public class SlippageToleranceService : ISlippageToleranceService
{
    private const decimal MIN_SLIPPAGE = 0.1m;  // 0.1%
    private const decimal MAX_SLIPPAGE = 50.0m; // 50%
    private const decimal EXCESSIVE_SLIPPAGE_THRESHOLD = 5.0m; // 5%

    private readonly ILogger<SlippageToleranceService> _logger;

    public SlippageToleranceService(ILogger<SlippageToleranceService> logger)
    {
        _logger = logger;
    }

    public decimal CalculateMinimumReceived(decimal expectedAmount, decimal slippagePercentage)
    {
        ValidateSlippage(slippagePercentage);

        if (expectedAmount <= 0)
        {
            throw new ArgumentException("Expected amount must be greater than 0", nameof(expectedAmount));
        }

        var slippageMultiplier = 1 - (slippagePercentage / 100m);
        var minimumReceived = expectedAmount * slippageMultiplier;

        // Round to 8 decimal places
        minimumReceived = Math.Round(minimumReceived, 8);

        _logger.LogDebug(
            "Calculated minimum received: Expected={Expected}, Slippage={Slippage}%, Minimum={Minimum}",
            expectedAmount,
            slippagePercentage,
            minimumReceived);

        return minimumReceived;
    }

    public void ValidateSlippage(decimal slippagePercentage)
    {
        if (slippagePercentage < MIN_SLIPPAGE || slippagePercentage > MAX_SLIPPAGE)
        {
            throw new ArgumentException(
                $"Slippage must be between {MIN_SLIPPAGE}% and {MAX_SLIPPAGE}%. Provided: {slippagePercentage}%",
                nameof(slippagePercentage));
        }
    }

    public bool IsSlippageExcessive(decimal slippagePercentage)
    {
        var isExcessive = slippagePercentage > EXCESSIVE_SLIPPAGE_THRESHOLD;

        if (isExcessive)
        {
            _logger.LogWarning(
                "Excessive slippage detected: {Slippage}% (threshold: {Threshold}%)",
                slippagePercentage,
                EXCESSIVE_SLIPPAGE_THRESHOLD);
        }

        return isExcessive;
    }

    public SlippageRecommendation GetRecommendedSlippage(
        string fromToken,
        string toToken,
        decimal amount)
    {
        decimal recommendedSlippage;
        string reason;

        // Recommend slippage based on trade size
        if (amount < 100m)
        {
            // Small trades (<$100): Low slippage
            recommendedSlippage = 0.5m;
            reason = "Low slippage recommended for small trades";
        }
        else if (amount < 1000m)
        {
            // Medium trades ($100-$1000): Standard slippage
            recommendedSlippage = 1.0m;
            reason = "Standard slippage for medium-sized trades";
        }
        else if (amount < 5000m)
        {
            // Large trades ($1000-$5000): Higher slippage
            recommendedSlippage = 2.0m;
            reason = "Higher slippage recommended for large trades";
        }
        else
        {
            // Very large trades (>$5000): Maximum reasonable slippage
            recommendedSlippage = 3.0m;
            reason = "High slippage recommended for very large trades to ensure execution";
        }

        _logger.LogInformation(
            "Slippage recommendation for {Amount} USD swap: {Slippage}% - {Reason}",
            amount,
            recommendedSlippage,
            reason);

        return new SlippageRecommendation
        {
            RecommendedSlippage = recommendedSlippage,
            Reason = reason,
            IsExcessive = IsSlippageExcessive(recommendedSlippage)
        };
    }
}
