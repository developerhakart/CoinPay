using CoinPay.Api.Services.Caching;
using System.Text.Json;

namespace CoinPay.Api.Services.FiatGateway;

/// <summary>
/// Service for managing exchange rates with caching
/// Fetches rates from Fiat Gateway and caches for 30 seconds
/// </summary>
public class ExchangeRateService : IExchangeRateService
{
    private readonly IFiatGatewayService _fiatGatewayService;
    private readonly ICachingService? _cachingService;
    private readonly ILogger<ExchangeRateService> _logger;

    private const string CacheKeyPrefix = "exchange_rate:";
    private const int CacheDurationSeconds = 30;

    public ExchangeRateService(
        IFiatGatewayService fiatGatewayService,
        ICachingService? cachingService,
        ILogger<ExchangeRateService> logger)
    {
        _fiatGatewayService = fiatGatewayService;
        _cachingService = cachingService;
        _logger = logger;
    }

    /// <summary>
    /// Get current exchange rate (cached)
    /// </summary>
    public async Task<ExchangeRateResponse> GetExchangeRateAsync(bool forceRefresh = false)
    {
        var cacheKey = $"{CacheKeyPrefix}USDC_USD";

        // Try cache first (unless force refresh)
        if (!forceRefresh && _cachingService != null)
        {
            var cachedJson = await _cachingService.GetAsync(cacheKey);
            if (cachedJson != null)
            {
                try
                {
                    var cached = JsonSerializer.Deserialize<ExchangeRateResponse>(cachedJson);
                    if (cached != null)
                    {
                        _logger.LogDebug("Exchange rate retrieved from cache");
                        return cached;
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cached exchange rate, fetching fresh");
                }
            }
        }

        // Fetch from gateway
        _logger.LogInformation("Fetching fresh exchange rate from gateway");
        var rate = await _fiatGatewayService.GetExchangeRateAsync();

        // Cache the result
        if (_cachingService != null)
        {
            try
            {
                var json = JsonSerializer.Serialize(rate);
                await _cachingService.SetAsync(cacheKey, json, TimeSpan.FromSeconds(CacheDurationSeconds));
                _logger.LogDebug("Exchange rate cached for {Seconds} seconds", CacheDurationSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to cache exchange rate");
            }
        }

        return rate;
    }

    /// <summary>
    /// Get conversion preview with current exchange rate
    /// </summary>
    public async Task<ConversionPreviewResponse> GetConversionPreviewAsync(decimal usdcAmount)
    {
        return await _fiatGatewayService.GetConversionPreviewAsync(usdcAmount);
    }
}

/// <summary>
/// Interface for exchange rate service
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Get current exchange rate (cached)
    /// </summary>
    /// <param name="forceRefresh">Force refresh from gateway</param>
    Task<ExchangeRateResponse> GetExchangeRateAsync(bool forceRefresh = false);

    /// <summary>
    /// Get conversion preview with fees
    /// </summary>
    Task<ConversionPreviewResponse> GetConversionPreviewAsync(decimal usdcAmount);
}
