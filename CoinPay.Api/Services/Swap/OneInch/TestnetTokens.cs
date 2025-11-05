namespace CoinPay.Api.Services.Swap.OneInch;

/// <summary>
/// Polygon Amoy testnet token addresses
/// </summary>
public static class TestnetTokens
{
    /// <summary>
    /// USDC token address on Polygon Amoy testnet
    /// </summary>
    public const string USDC = "0x41e94eb019c0762f9bfcf9fb1e58725bfb0e7582";

    /// <summary>
    /// Wrapped ETH token address on Polygon Amoy testnet
    /// </summary>
    public const string WETH = "0x360ad4f9a9A8EFe9A8DCB5f461c4Cc1047E1Dcf9";

    /// <summary>
    /// Wrapped MATIC token address on Polygon Amoy testnet
    /// </summary>
    public const string WMATIC = "0x0d500B1d8E8eF31E21C99d1Db9A6444d3ADf1270";

    /// <summary>
    /// Native MATIC address (used for native currency swaps)
    /// </summary>
    public const string NATIVE_MATIC = "0xEeeeeEeeeEeEeeEeEeEeeEEEeeeeEeeeeeeeEEeE";

    /// <summary>
    /// Gets the token symbol for an address
    /// </summary>
    public static string GetSymbol(string address)
    {
        return address.ToLower() switch
        {
            var a when a == USDC.ToLower() => "USDC",
            var a when a == WETH.ToLower() => "WETH",
            var a when a == WMATIC.ToLower() => "WMATIC",
            var a when a == NATIVE_MATIC.ToLower() => "MATIC",
            _ => "UNKNOWN"
        };
    }

    /// <summary>
    /// Gets the decimals for a token
    /// </summary>
    public static int GetDecimals(string address)
    {
        return address.ToLower() switch
        {
            var a when a == USDC.ToLower() => 6,  // USDC has 6 decimals
            var a when a == WETH.ToLower() => 18,
            var a when a == WMATIC.ToLower() => 18,
            var a when a == NATIVE_MATIC.ToLower() => 18,
            _ => 18 // Default to 18
        };
    }
}
