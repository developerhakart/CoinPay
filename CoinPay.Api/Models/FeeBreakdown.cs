namespace CoinPay.Api.Models;

/// <summary>
/// Breakdown of fees for a swap transaction
/// </summary>
public class FeeBreakdown
{
    /// <summary>
    /// Platform fee amount in source token
    /// </summary>
    public decimal PlatformFee { get; set; }

    /// <summary>
    /// Platform fee percentage (e.g., 0.5 for 0.5%)
    /// </summary>
    public decimal PlatformFeePercentage { get; set; }

    /// <summary>
    /// DEX protocol fee (if applicable)
    /// </summary>
    public decimal DexFee { get; set; }

    /// <summary>
    /// Total fee amount
    /// </summary>
    public decimal TotalFee { get; set; }

    /// <summary>
    /// Fee token address
    /// </summary>
    public string FeeToken { get; set; } = string.Empty;
}
