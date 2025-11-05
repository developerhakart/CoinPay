namespace CoinPay.Api.Models;

/// <summary>
/// Complete swap quote result with fees and price impact
/// </summary>
public class SwapQuoteResult
{
    /// <summary>
    /// Source token address
    /// </summary>
    public string FromToken { get; set; } = string.Empty;

    /// <summary>
    /// Source token symbol
    /// </summary>
    public string FromTokenSymbol { get; set; } = string.Empty;

    /// <summary>
    /// Destination token address
    /// </summary>
    public string ToToken { get; set; } = string.Empty;

    /// <summary>
    /// Destination token symbol
    /// </summary>
    public string ToTokenSymbol { get; set; } = string.Empty;

    /// <summary>
    /// Amount of source token to swap
    /// </summary>
    public decimal FromAmount { get; set; }

    /// <summary>
    /// Amount of destination token to receive
    /// </summary>
    public decimal ToAmount { get; set; }

    /// <summary>
    /// Exchange rate (toToken per fromToken)
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Platform fee amount in source token
    /// </summary>
    public decimal PlatformFee { get; set; }

    /// <summary>
    /// Platform fee percentage (e.g., 0.5 for 0.5%)
    /// </summary>
    public decimal PlatformFeePercentage { get; set; }

    /// <summary>
    /// Estimated gas units
    /// </summary>
    public string EstimatedGas { get; set; } = string.Empty;

    /// <summary>
    /// Estimated gas cost in native token (MATIC)
    /// </summary>
    public decimal EstimatedGasCost { get; set; }

    /// <summary>
    /// Price impact percentage
    /// </summary>
    public decimal PriceImpact { get; set; }

    /// <summary>
    /// Slippage tolerance percentage
    /// </summary>
    public decimal SlippageTolerance { get; set; }

    /// <summary>
    /// Minimum amount to receive after slippage
    /// </summary>
    public decimal MinimumReceived { get; set; }

    /// <summary>
    /// Quote expiry timestamp
    /// </summary>
    public DateTime QuoteValidUntil { get; set; }

    /// <summary>
    /// DEX provider name
    /// </summary>
    public string Provider { get; set; } = string.Empty;
}
