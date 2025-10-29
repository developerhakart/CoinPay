namespace CoinPay.Api.Services.Caching;

/// <summary>
/// Service for caching operations using Redis
/// </summary>
public interface ICachingService
{
    /// <summary>
    /// Gets a cached value by key
    /// </summary>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// Sets a value in cache with expiration
    /// </summary>
    Task SetAsync(string key, string value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a value from cache
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// Checks if a key exists in cache
    /// </summary>
    Task<bool> ExistsAsync(string key);
}
