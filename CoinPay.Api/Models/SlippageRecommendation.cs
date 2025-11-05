namespace CoinPay.Api.Models;

/// <summary>
/// Recommended slippage tolerance for a swap
/// </summary>
public class SlippageRecommendation
{
    /// <summary>
    /// Recommended slippage percentage
    /// </summary>
    public decimal RecommendedSlippage { get; set; }

    /// <summary>
    /// Reason for the recommendation
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Whether the current slippage is excessive
    /// </summary>
    public bool IsExcessive { get; set; }
}
