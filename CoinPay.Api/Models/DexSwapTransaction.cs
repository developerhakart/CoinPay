namespace CoinPay.Api.Models;

/// <summary>
/// Represents a swap transaction data from DEX aggregator
/// </summary>
public class DexSwapTransaction
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
    /// Amount of source token in wei/smallest unit
    /// </summary>
    public string FromTokenAmount { get; set; } = string.Empty;

    /// <summary>
    /// Expected amount of destination token in wei/smallest unit
    /// </summary>
    public string ToTokenAmount { get; set; } = string.Empty;

    /// <summary>
    /// Exchange rate
    /// </summary>
    public decimal ExchangeRate { get; set; }

    /// <summary>
    /// Platform fee amount
    /// </summary>
    public decimal PlatformFee { get; set; }

    /// <summary>
    /// Minimum amount to receive after slippage
    /// </summary>
    public decimal MinimumReceived { get; set; }

    /// <summary>
    /// Contract address to send transaction to
    /// </summary>
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Transaction data (encoded function call)
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Value in native currency (for native token swaps)
    /// </summary>
    public string Value { get; set; } = "0";

    /// <summary>
    /// Estimated gas units
    /// </summary>
    public string Gas { get; set; } = string.Empty;

    /// <summary>
    /// Gas price in wei
    /// </summary>
    public string? GasPrice { get; set; }

    /// <summary>
    /// Spender address (router contract) that needs token approval
    /// </summary>
    public string SpenderAddress { get; set; } = string.Empty;
}
