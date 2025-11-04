namespace CoinPay.Api.Services.ExchangeRate;

/// <summary>
/// Service for managing cryptocurrency to fiat exchange rates
/// Provides caching and real-time rate updates
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Get current USDC to USD exchange rate
    /// </summary>
    /// <returns>Current exchange rate with metadata</returns>
    Task<ExchangeRateInfo> GetUsdcToUsdRateAsync();

    /// <summary>
    /// Get cached rate if available, otherwise fetch new rate
    /// </summary>
    /// <param name="maxAgeSeconds">Maximum age of cached rate in seconds</param>
    /// <returns>Exchange rate information</returns>
    Task<ExchangeRateInfo> GetCachedRateAsync(int maxAgeSeconds = 30);

    /// <summary>
    /// Force refresh of exchange rate from source
    /// </summary>
    /// <returns>Newly fetched exchange rate</returns>
    Task<ExchangeRateInfo> RefreshRateAsync();

    /// <summary>
    /// Convert USDC amount to USD using current rate
    /// </summary>
    /// <param name="usdcAmount">Amount in USDC</param>
    /// <returns>Equivalent USD amount</returns>
    Task<decimal> ConvertUsdcToUsdAsync(decimal usdcAmount);

    /// <summary>
    /// Check if exchange rate service is available
    /// </summary>
    /// <returns>True if service is healthy</returns>
    Task<bool> IsAvailableAsync();
}

/// <summary>
/// Exchange rate information with metadata
/// </summary>
public class ExchangeRateInfo
{
    /// <summary>
    /// Exchange rate value (e.g., 0.9998 means 1 USDC = 0.9998 USD)
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Base currency (USDC)
    /// </summary>
    public string BaseCurrency { get; set; } = "USDC";

    /// <summary>
    /// Quote currency (USD)
    /// </summary>
    public string QuoteCurrency { get; set; } = "USD";

    /// <summary>
    /// When this rate was fetched
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// How long this rate is valid (seconds)
    /// </summary>
    public int ValidForSeconds { get; set; }

    /// <summary>
    /// Rate source identifier (e.g., "CoinGecko", "RedotPay", "Mock")
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Is this rate from cache?
    /// </summary>
    public bool IsCached { get; set; }

    /// <summary>
    /// When the rate expires
    /// </summary>
    public DateTime ExpiresAt => Timestamp.AddSeconds(ValidForSeconds);

    /// <summary>
    /// Is the rate still valid?
    /// </summary>
    public bool IsValid => DateTime.UtcNow < ExpiresAt;

    /// <summary>
    /// Seconds until expiration
    /// </summary>
    public int SecondsUntilExpiration => Math.Max(0, (int)(ExpiresAt - DateTime.UtcNow).TotalSeconds);
}
