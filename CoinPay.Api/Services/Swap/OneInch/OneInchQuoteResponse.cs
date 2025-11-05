using System.Text.Json.Serialization;

namespace CoinPay.Api.Services.Swap.OneInch;

/// <summary>
/// Response from 1inch /quote endpoint
/// </summary>
public class OneInchQuoteResponse
{
    [JsonPropertyName("fromToken")]
    public OneInchTokenInfo FromToken { get; set; } = new();

    [JsonPropertyName("toToken")]
    public OneInchTokenInfo ToToken { get; set; } = new();

    [JsonPropertyName("fromTokenAmount")]
    public string FromTokenAmount { get; set; } = string.Empty;

    [JsonPropertyName("toTokenAmount")]
    public string ToTokenAmount { get; set; } = string.Empty;

    [JsonPropertyName("estimatedGas")]
    public long EstimatedGas { get; set; }
}

/// <summary>
/// Token information in 1inch response
/// </summary>
public class OneInchTokenInfo
{
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("decimals")]
    public int Decimals { get; set; }
}
