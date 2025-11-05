using System.Text.Json.Serialization;

namespace CoinPay.Api.Services.Swap.OneInch;

/// <summary>
/// Response from 1inch /swap endpoint
/// </summary>
public class OneInchSwapResponse
{
    [JsonPropertyName("fromToken")]
    public OneInchTokenInfo FromToken { get; set; } = new();

    [JsonPropertyName("toToken")]
    public OneInchTokenInfo ToToken { get; set; } = new();

    [JsonPropertyName("fromTokenAmount")]
    public string FromTokenAmount { get; set; } = string.Empty;

    [JsonPropertyName("toTokenAmount")]
    public string ToTokenAmount { get; set; } = string.Empty;

    [JsonPropertyName("tx")]
    public OneInchTransactionData Tx { get; set; } = new();
}

/// <summary>
/// Transaction data from 1inch swap response
/// </summary>
public class OneInchTransactionData
{
    [JsonPropertyName("from")]
    public string From { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    public string To { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = "0";

    [JsonPropertyName("gas")]
    public long Gas { get; set; }

    [JsonPropertyName("gasPrice")]
    public string GasPrice { get; set; } = string.Empty;
}
