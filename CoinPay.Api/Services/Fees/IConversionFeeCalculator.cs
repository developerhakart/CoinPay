namespace CoinPay.Api.Services.Fees;

/// <summary>
/// Service for calculating conversion and payout fees
/// Centralizes fee logic for transparency and maintainability
/// </summary>
public interface IConversionFeeCalculator
{
    /// <summary>
    /// Calculate total fees for a USDC to USD conversion
    /// </summary>
    /// <param name="usdAmount">USD amount before fees</param>
    /// <returns>Fee breakdown</returns>
    FeeBreakdown CalculateConversionFees(decimal usdAmount);

    /// <summary>
    /// Calculate payout fee only
    /// </summary>
    /// <param name="usdAmount">USD amount</param>
    /// <returns>Payout fee amount</returns>
    decimal CalculatePayoutFee(decimal usdAmount);

    /// <summary>
    /// Calculate conversion fee only
    /// </summary>
    /// <param name="usdAmount">USD amount</param>
    /// <returns>Conversion fee amount</returns>
    decimal CalculateConversionFee(decimal usdAmount);

    /// <summary>
    /// Get current fee configuration
    /// </summary>
    /// <returns>Fee configuration details</returns>
    FeeConfiguration GetFeeConfiguration();

    /// <summary>
    /// Calculate net amount after all fees
    /// </summary>
    /// <param name="usdAmount">USD amount before fees</param>
    /// <returns>Net amount user will receive</returns>
    decimal CalculateNetAmount(decimal usdAmount);
}

/// <summary>
/// Complete fee breakdown for a conversion
/// </summary>
public class FeeBreakdown
{
    /// <summary>
    /// Original USD amount before fees
    /// </summary>
    public decimal UsdAmountBeforeFees { get; set; }

    /// <summary>
    /// Conversion fee percentage (e.g., 1.5 for 1.5%)
    /// </summary>
    public decimal ConversionFeePercent { get; set; }

    /// <summary>
    /// Conversion fee amount in USD
    /// </summary>
    public decimal ConversionFeeAmount { get; set; }

    /// <summary>
    /// Flat payout fee in USD
    /// </summary>
    public decimal PayoutFeeAmount { get; set; }

    /// <summary>
    /// Total fees (conversion + payout)
    /// </summary>
    public decimal TotalFees { get; set; }

    /// <summary>
    /// Net amount after all fees
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Effective fee rate as percentage of original amount
    /// </summary>
    public decimal EffectiveFeeRate => UsdAmountBeforeFees > 0
        ? (TotalFees / UsdAmountBeforeFees) * 100
        : 0;
}

/// <summary>
/// Fee configuration details
/// </summary>
public class FeeConfiguration
{
    /// <summary>
    /// Conversion fee percentage (e.g., 1.5 for 1.5%)
    /// </summary>
    public decimal ConversionFeePercent { get; set; }

    /// <summary>
    /// Flat payout fee in USD
    /// </summary>
    public decimal PayoutFlatFee { get; set; }

    /// <summary>
    /// Minimum payout amount to avoid excessive fees
    /// </summary>
    public decimal MinimumPayoutAmount { get; set; }

    /// <summary>
    /// Maximum payout amount (if any)
    /// </summary>
    public decimal? MaximumPayoutAmount { get; set; }

    /// <summary>
    /// Fee tier information (for future tiered pricing)
    /// </summary>
    public string FeeTier { get; set; } = "Standard";

    /// <summary>
    /// Description of fee structure
    /// </summary>
    public string Description => $"{ConversionFeePercent}% conversion fee + ${PayoutFlatFee} payout fee";
}
