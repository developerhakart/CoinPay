namespace CoinPay.Api.Models;

/// <summary>
/// Represents a swap quote from a DEX aggregator
/// </summary>
public class SwapQuote
{
    /// <summary>
    /// Source token address
    /// </summary>
    public string FromToken { get; set; } = string.Empty;

    /// <summary>
    /// Destination token address
    /// </summary>
    public string ToToken { get; set; } = string.Empty;

    /// <summary>
    /// Amount of source token
    /// </summary>
    public decimal FromTokenAmount { get; set; }

    /// <summary>
    /// Amount of destination token to receive
    /// </summary>
    public decimal ToTokenAmount { get; set; }

    /// <summary>
    /// Exchange rate (toToken per fromToken)
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Estimated gas units for the swap
    /// </summary>
    public string EstimatedGas { get; set; } = string.Empty;

    /// <summary>
    /// DEX provider name (e.g., "1inch", "0x")
    /// </summary>
    public string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Quote timestamp
    /// </summary>
    public DateTime QuotedAt { get; set; } = DateTime.UtcNow;
}
