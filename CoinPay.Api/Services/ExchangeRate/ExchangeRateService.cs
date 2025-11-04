using Microsoft.Extensions.Caching.Memory;

namespace CoinPay.Api.Services.ExchangeRate;

/// <summary>
/// Exchange rate service implementation with in-memory caching
/// In production, this would integrate with real APIs like CoinGecko, CoinMarketCap, or RedotPay
/// </summary>
public class ExchangeRateService : IExchangeRateService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<ExchangeRateService> _logger;
    private readonly IConfiguration _configuration;

    private const string CacheKey = "ExchangeRate_USDC_USD";
    private const decimal MockRate = 0.9998m; // Mock: 1 USDC ≈ 0.9998 USD
    private const int DefaultValidSeconds = 30;

    public ExchangeRateService(
        IMemoryCache cache,
        ILogger<ExchangeRateService> logger,
        IConfiguration configuration)
    {
        _cache = cache;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Get current USDC to USD exchange rate
    /// Uses cache with 30-second TTL
    /// </summary>
    public async Task<ExchangeRateInfo> GetUsdcToUsdRateAsync()
    {
        return await GetCachedRateAsync(DefaultValidSeconds);
    }

    /// <summary>
    /// Get cached rate if available and not expired
    /// </summary>
    public async Task<ExchangeRateInfo> GetCachedRateAsync(int maxAgeSeconds = 30)
    {
        // Try to get from cache
        if (_cache.TryGetValue(CacheKey, out ExchangeRateInfo? cachedRate))
        {
            if (cachedRate != null && cachedRate.IsValid)
            {
                _logger.LogDebug("Returning cached exchange rate: {Rate} (expires in {Seconds}s)",
                    cachedRate.Rate, cachedRate.SecondsUntilExpiration);

                cachedRate.IsCached = true;
                return cachedRate;
            }
        }

        // Cache miss or expired - fetch new rate
        _logger.LogInformation("Exchange rate cache miss or expired. Fetching new rate...");
        return await RefreshRateAsync();
    }

    /// <summary>
    /// Force refresh exchange rate from source
    /// In production, this would call a real API
    /// </summary>
    public async Task<ExchangeRateInfo> RefreshRateAsync()
    {
        _logger.LogInformation("Fetching exchange rate from source");

        // Simulate API call delay
        await Task.Delay(50);

        // In production, this would call:
        // - CoinGecko API: https://api.coingecko.com/api/v3/simple/price?ids=usd-coin&vs_currencies=usd
        // - CoinMarketCap API
        // - RedotPay Gateway API
        // - Or any other exchange rate provider

        var rate = new ExchangeRateInfo
        {
            Rate = GetRateFromSource(),
            BaseCurrency = "USDC",
            QuoteCurrency = "USD",
            Timestamp = DateTime.UtcNow,
            ValidForSeconds = GetValidityPeriod(),
            Source = GetRateSource(),
            IsCached = false
        };

        // Store in cache
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(rate.ValidForSeconds),
            Priority = CacheItemPriority.High
        };

        _cache.Set(CacheKey, rate, cacheOptions);

        _logger.LogInformation("Exchange rate updated: {Rate} {BaseCurrency}/{QuoteCurrency} from {Source} (valid for {Seconds}s)",
            rate.Rate, rate.BaseCurrency, rate.QuoteCurrency, rate.Source, rate.ValidForSeconds);

        return rate;
    }

    /// <summary>
    /// Convert USDC to USD using current exchange rate
    /// </summary>
    public async Task<decimal> ConvertUsdcToUsdAsync(decimal usdcAmount)
    {
        var rateInfo = await GetUsdcToUsdRateAsync();
        var usdAmount = usdcAmount * rateInfo.Rate;

        _logger.LogDebug("Converted {UsdcAmount} USDC to {UsdAmount} USD at rate {Rate}",
            usdcAmount, usdAmount, rateInfo.Rate);

        return usdAmount;
    }

    /// <summary>
    /// Check service availability
    /// </summary>
    public Task<bool> IsAvailableAsync()
    {
        // In production, this would ping the actual API endpoint
        _logger.LogDebug("Exchange rate service availability check: OK");
        return Task.FromResult(true);
    }

    #region Private Helper Methods

    /// <summary>
    /// Get exchange rate from configured source
    /// In production, this would make actual API calls
    /// </summary>
    private decimal GetRateFromSource()
    {
        // Check configuration for rate source
        var rateSource = _configuration.GetValue<string>("ExchangeRate:Source") ?? "Mock";

        switch (rateSource.ToLowerInvariant())
        {
            case "coingecko":
                return FetchFromCoinGecko();

            case "coinmarketcap":
                return FetchFromCoinMarketCap();

            case "redotpay":
                return FetchFromRedotPay();

            case "mock":
            default:
                return MockRate;
        }
    }

    /// <summary>
    /// Get rate validity period from configuration
    /// </summary>
    private int GetValidityPeriod()
    {
        return _configuration.GetValue<int>("ExchangeRate:ValiditySeconds", DefaultValidSeconds);
    }

    /// <summary>
    /// Get rate source name from configuration
    /// </summary>
    private string GetRateSource()
    {
        return _configuration.GetValue<string>("ExchangeRate:Source") ?? "Mock";
    }

    #region API Integration Stubs (for future implementation)

    /// <summary>
    /// Fetch rate from CoinGecko API
    /// TODO: Implement actual API call
    /// </summary>
    private decimal FetchFromCoinGecko()
    {
        _logger.LogInformation("CoinGecko integration not yet implemented. Using mock rate.");
        // Future implementation:
        // var response = await _httpClient.GetAsync("https://api.coingecko.com/api/v3/simple/price?ids=usd-coin&vs_currencies=usd");
        // var data = await response.Content.ReadFromJsonAsync<CoinGeckoResponse>();
        // return data.UsdCoin.Usd;
        return MockRate;
    }

    /// <summary>
    /// Fetch rate from CoinMarketCap API
    /// TODO: Implement actual API call
    /// </summary>
    private decimal FetchFromCoinMarketCap()
    {
        _logger.LogInformation("CoinMarketCap integration not yet implemented. Using mock rate.");
        return MockRate;
    }

    /// <summary>
    /// Fetch rate from RedotPay Gateway
    /// TODO: Implement actual API call
    /// </summary>
    private decimal FetchFromRedotPay()
    {
        _logger.LogInformation("RedotPay integration not yet implemented. Using mock rate.");
        return MockRate;
    }

    #endregion

    #endregion
}
