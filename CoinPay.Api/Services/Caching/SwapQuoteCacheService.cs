using CoinPay.Api.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoinPay.Api.Services.Caching;

/// <summary>
/// Redis-based caching service for swap quotes
/// </summary>
public class SwapQuoteCacheService : ISwapQuoteCacheService
{
    private readonly IDistributedCache? _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SwapQuoteCacheService> _logger;

    private int CacheTtlSeconds => _configuration.GetValue<int>("Swap:CacheTTLSeconds", 30);

    public SwapQuoteCacheService(
        IDistributedCache? cache,
        IConfiguration configuration,
        ILogger<SwapQuoteCacheService> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;

        if (_cache == null)
        {
            _logger.LogWarning("Distributed cache not available. Quote caching will be disabled.");
        }
    }

    public async Task<SwapQuoteResult?> GetCachedQuoteAsync(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippage)
    {
        if (_cache == null)
        {
            return null;
        }

        try
        {
            var cacheKey = BuildCacheKey(fromToken, toToken, amount, slippage);
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData != null)
            {
                _logger.LogInformation("Cache HIT for swap quote: {CacheKey}", cacheKey);

                var quote = JsonSerializer.Deserialize<SwapQuoteResult>(cachedData);

                // Verify quote hasn't expired
                if (quote != null && quote.QuoteValidUntil > DateTime.UtcNow)
                {
                    return quote;
                }
                else
                {
                    _logger.LogDebug("Cached quote expired, invalidating");
                    await _cache.RemoveAsync(cacheKey);
                }
            }
            else
            {
                _logger.LogDebug("Cache MISS for swap quote: {CacheKey}", cacheKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cached swap quote");
        }

        return null;
    }

    public async Task SetCachedQuoteAsync(
        SwapQuoteResult quote,
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippage)
    {
        if (_cache == null)
        {
            return;
        }

        try
        {
            var cacheKey = BuildCacheKey(fromToken, toToken, amount, slippage);
            var serialized = JsonSerializer.Serialize(quote);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheTtlSeconds)
            };

            await _cache.SetStringAsync(cacheKey, serialized, options);

            _logger.LogInformation(
                "Cached swap quote: {CacheKey}, TTL: {TTL}s",
                cacheKey,
                CacheTtlSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error caching swap quote");
            // Don't throw - caching failure shouldn't break quote fetching
        }
    }

    public async Task InvalidateCacheAsync(string fromToken, string toToken)
    {
        if (_cache == null)
        {
            return;
        }

        try
        {
            // Note: This is a simple implementation that doesn't track all possible cache keys
            // In production, consider using Redis pattern matching or cache key tracking
            _logger.LogInformation(
                "Cache invalidation requested for pair: {FromToken} -> {ToToken}",
                fromToken,
                toToken);

            // For now, just log - individual quotes will expire naturally
            // A full implementation would track and remove all related keys
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating cache");
        }
    }

    private string BuildCacheKey(
        string fromToken,
        string toToken,
        decimal amount,
        decimal slippage)
    {
        // Normalize addresses to lowercase
        var from = fromToken.ToLower();
        var to = toToken.ToLower();

        // Round amount to 6 decimals for cache key consistency
        var amountKey = amount.ToString("F6");

        // Round slippage to 1 decimal
        var slippageKey = slippage.ToString("F1");

        return $"swap:quote:{from}:{to}:{amountKey}:{slippageKey}";
    }
}
