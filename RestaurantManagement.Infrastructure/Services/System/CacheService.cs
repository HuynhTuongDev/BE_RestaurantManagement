namespace RestaurantManagement.Infrastructure.Services.System;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

/// <summary>
/// Interface for caching service
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get value from cache
    /// </summary>
    T? Get<T>(string key);

    /// <summary>
    /// Get value from cache asynchronously
    /// </summary>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Set value in cache
    /// </summary>
    void Set<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Set value in cache asynchronously
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Remove value from cache
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// Remove value from cache asynchronously
    /// </summary>
    Task RemoveAsync(string key);
}

/// <summary>
/// In-memory cache service implementation
/// </summary>
public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;
    private const int DefaultExpirationHours = 1;

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            _logger.LogInformation("Cache HIT: {CacheKey}", key);
            return value;
        }
        
        _logger.LogInformation("Cache MISS: {CacheKey}", key);
        return default;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        return await Task.FromResult(Get<T>(key));
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(DefaultExpirationHours)
        };

        _cache.Set(key, value, cacheOptions);
        _logger.LogInformation("Cache SET: {CacheKey} (Expiration: {Expiration})", key, cacheOptions.AbsoluteExpirationRelativeToNow);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        Set(key, value, expiration);
        await Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _logger.LogInformation("Cache REMOVED: {CacheKey}", key);
    }

    public async Task RemoveAsync(string key)
    {
        Remove(key);
        await Task.CompletedTask;
    }
}
