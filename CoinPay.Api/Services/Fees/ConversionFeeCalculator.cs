namespace CoinPay.Api.Services.Fees;

/// <summary>
/// Implementation of conversion fee calculator
/// Uses configuration-driven fee structure
/// </summary>
public class ConversionFeeCalculator : IConversionFeeCalculator
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConversionFeeCalculator> _logger;

    // Default fee structure (can be overridden via configuration)
    private const decimal DefaultConversionFeePercent = 1.5m; // 1.5%
    private const decimal DefaultPayoutFlatFee = 1.00m; // $1.00
    private const decimal DefaultMinimumPayout = 10.00m; // $10.00 minimum

    public ConversionFeeCalculator(
        IConfiguration configuration,
        ILogger<ConversionFeeCalculator> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Calculate complete fee breakdown
    /// </summary>
    public FeeBreakdown CalculateConversionFees(decimal usdAmount)
    {
        var config = GetFeeConfiguration();

        var conversionFee = CalculateConversionFee(usdAmount);
        var payoutFee = CalculatePayoutFee(usdAmount);
        var totalFees = conversionFee + payoutFee;
        var netAmount = usdAmount - totalFees;

        var breakdown = new FeeBreakdown
        {
            UsdAmountBeforeFees = usdAmount,
            ConversionFeePercent = config.ConversionFeePercent,
            ConversionFeeAmount = conversionFee,
            PayoutFeeAmount = payoutFee,
            TotalFees = totalFees,
            NetAmount = netAmount
        };

        _logger.LogDebug("Fee calculation for ${UsdAmount}: Conversion=${ConversionFee}, Payout=${PayoutFee}, Total=${TotalFees}, Net=${NetAmount}",
            usdAmount, conversionFee, payoutFee, totalFees, netAmount);

        return breakdown;
    }

    /// <summary>
    /// Calculate conversion fee based on percentage
    /// </summary>
    public decimal CalculateConversionFee(decimal usdAmount)
    {
        var feePercent = GetConversionFeePercent();
        var fee = usdAmount * (feePercent / 100m);

        return Math.Round(fee, 2);
    }

    /// <summary>
    /// Calculate flat payout fee
    /// </summary>
    public decimal CalculatePayoutFee(decimal usdAmount)
    {
        // Payout fee is flat rate, but could be tiered in future
        var fee = GetPayoutFlatFee();

        return Math.Round(fee, 2);
    }

    /// <summary>
    /// Calculate net amount after all fees
    /// </summary>
    public decimal CalculateNetAmount(decimal usdAmount)
    {
        var totalFees = CalculateConversionFee(usdAmount) + CalculatePayoutFee(usdAmount);
        return Math.Round(usdAmount - totalFees, 2);
    }

    /// <summary>
    /// Get current fee configuration
    /// </summary>
    public FeeConfiguration GetFeeConfiguration()
    {
        return new FeeConfiguration
        {
            ConversionFeePercent = GetConversionFeePercent(),
            PayoutFlatFee = GetPayoutFlatFee(),
            MinimumPayoutAmount = GetMinimumPayoutAmount(),
            MaximumPayoutAmount = GetMaximumPayoutAmount(),
            FeeTier = "Standard"
        };
    }

    #region Private Configuration Helpers

    /// <summary>
    /// Get conversion fee percentage from configuration
    /// </summary>
    private decimal GetConversionFeePercent()
    {
        return _configuration.GetValue<decimal>("Fees:ConversionFeePercent", DefaultConversionFeePercent);
    }

    /// <summary>
    /// Get payout flat fee from configuration
    /// </summary>
    private decimal GetPayoutFlatFee()
    {
        return _configuration.GetValue<decimal>("Fees:PayoutFlatFee", DefaultPayoutFlatFee);
    }

    /// <summary>
    /// Get minimum payout amount from configuration
    /// </summary>
    private decimal GetMinimumPayoutAmount()
    {
        return _configuration.GetValue<decimal>("Fees:MinimumPayoutAmount", DefaultMinimumPayout);
    }

    /// <summary>
    /// Get maximum payout amount from configuration (optional)
    /// </summary>
    private decimal? GetMaximumPayoutAmount()
    {
        var maxPayout = _configuration.GetValue<decimal?>("Fees:MaximumPayoutAmount");
        return maxPayout;
    }

    #endregion
}
