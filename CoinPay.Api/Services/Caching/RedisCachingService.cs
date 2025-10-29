using StackExchange.Redis;

namespace CoinPay.Api.Services.Caching;

/// <summary>
/// Redis implementation of caching service
/// </summary>
public class RedisCachingService : ICachingService
{
    private readonly IDatabase _redis;
    private readonly ILogger<RedisCachingService> _logger;

    public RedisCachingService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCachingService> logger)
    {
        _redis = connectionMultiplexer.GetDatabase();
        _logger = logger;
    }

    public async Task<string?> GetAsync(string key)
    {
        try
        {
            var value = await _redis.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync(string key, string value, TimeSpan? expiration = null)
    {
        try
        {
            await _redis.StringSetAsync(key, value, expiration);
            _logger.LogDebug("Cache set: {Key}, Expiration: {Expiration}s", key, expiration?.TotalSeconds ?? -1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _redis.KeyDeleteAsync(key);
            _logger.LogDebug("Cache removed: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _redis.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache key existence: {Key}", key);
            return false;
        }
    }
}
