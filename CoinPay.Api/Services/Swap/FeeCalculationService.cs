using CoinPay.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoinPay.Api.Services.Swap;

/// <summary>
/// Service for calculating platform swap fees
/// </summary>
public class FeeCalculationService : IFeeCalculationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FeeCalculationService> _logger;

    private const decimal STANDARD_FEE_PERCENTAGE = 0.5m; // 0.5%
    private const decimal HIGH_VOLUME_FEE_PERCENTAGE = 0.3m; // 0.3% for high volume users
    private const decimal HIGH_VOLUME_THRESHOLD = 10000m; // $10k monthly volume threshold

    private decimal PlatformFeePercentage =>
        _configuration.GetValue<decimal>("Swap:PlatformFeePercentage", STANDARD_FEE_PERCENTAGE);

    public FeeCalculationService(
        IConfiguration configuration,
        ILogger<FeeCalculationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<decimal> CalculateSwapFeeAsync(string tokenAddress, decimal amount)
    {
        if (amount <= 0)
        {
            return Task.FromResult(0m);
        }

        var feePercentage = PlatformFeePercentage;
        var fee = amount * (feePercentage / 100m);

        // Round to 8 decimal places for precision
        fee = Math.Round(fee, 8);

        _logger.LogDebug(
            "Calculated swap fee: Amount={Amount}, FeePercentage={FeePercentage}%, Fee={Fee}",
            amount,
            feePercentage,
            fee);

        return Task.FromResult(fee);
    }

    public async Task<FeeBreakdown> GetFeeBreakdownAsync(
        string fromToken,
        string toToken,
        decimal fromAmount)
    {
        var platformFee = await CalculateSwapFeeAsync(fromToken, fromAmount);
        var platformFeePercentage = PlatformFeePercentage;

        // 1inch doesn't charge a separate protocol fee
        var dexFee = 0m;

        var breakdown = new FeeBreakdown
        {
            PlatformFee = platformFee,
            PlatformFeePercentage = platformFeePercentage,
            DexFee = dexFee,
            TotalFee = platformFee + dexFee,
            FeeToken = fromToken
        };

        _logger.LogInformation(
            "Fee breakdown: Platform={PlatformFee} ({PlatformFeePercentage}%), DEX={DexFee}, Total={TotalFee}",
            breakdown.PlatformFee,
            breakdown.PlatformFeePercentage,
            breakdown.DexFee,
            breakdown.TotalFee);

        return breakdown;
    }

    public Task<decimal> GetFeePercentageAsync(Guid? userId = null)
    {
        // For MVP, return standard fee
        // In future, implement volume-based fee tiers:
        // - Check user's monthly swap volume
        // - Apply discounted fee for high-volume users

        var feePercentage = PlatformFeePercentage;

        _logger.LogDebug("Fee percentage for user {UserId}: {FeePercentage}%",
            userId?.ToString() ?? "anonymous",
            feePercentage);

        return Task.FromResult(feePercentage);
    }
}
