namespace RestaurantManagement.Infrastructure.Services.System;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Services.System;

/// <summary>
/// Cache service implementation using IMemoryCache
/// </summary>
public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromHours(1);

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Get cached value
    /// </summary>
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            if (_cache.TryGetValue(key, out string? cachedValue) && !string.IsNullOrEmpty(cachedValue))
            {
                _logger.LogDebug("Cache HIT for key: {Key}", key);
                // Offload deserialization to a background thread to justify async
                return await Task.Run(() => global::System.Text.Json.JsonSerializer.Deserialize<T>(cachedValue));
            }

            _logger.LogDebug("Cache MISS for key: {Key}", key);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached value for key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// Set cache value
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            var serializedValue = global::System.Text.Json.JsonSerializer.Serialize(value);
            var cacheExpiration = expiration ?? _defaultExpiration;

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheExpiration
            };

            _cache.Set(key, serializedValue, cacheOptions);
            _logger.LogDebug("Cached value for key: {Key} with expiration: {Expiration}", key, cacheExpiration);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cached value for key: {Key}", key);
        }
    }

    /// <summary>
    /// Remove cached value
    /// </summary>
    public async Task RemoveAsync(string key)
    {
        try
        {
            _cache.Remove(key);
            _logger.LogDebug("Removed cached value for key: {Key}", key);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cached value for key: {Key}", key);
        }
    }

    /// <summary>
    /// Check if key exists in cache
    /// </summary>
    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var exists = _cache.TryGetValue(key, out _);
            await Task.CompletedTask;
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence for key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Clear all cached values (not supported by IMemoryCache)
    /// </summary>
    public async Task ClearAsync()
    {
        _logger.LogWarning("ClearAsync is not supported by IMemoryCache. Cache will be cleared on app restart.");
        await Task.CompletedTask;
    }

    /// <summary>
    /// Remove cached values by pattern (not supported by IMemoryCache)
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern)
    {
        _logger.LogWarning("RemoveByPatternAsync is not supported by IMemoryCache. Use specific keys instead.");
        await Task.CompletedTask;
    }
}
