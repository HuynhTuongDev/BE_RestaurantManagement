namespace RestaurantManagement.Application.Services.System;

/// <summary>
/// Cache service interface for caching operations
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get cached value
    /// </summary>
    /// <typeparam name="T">Type of cached value</typeparam>
    /// <param name="key">Cache key</param>
    /// <returns>Cached value or default</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Set cache value
    /// </summary>
    /// <typeparam name="T">Type of value to cache</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <param name="expiration">Expiration time (optional)</param>
    /// <returns>Task</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Remove cached value
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>Task</returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Check if key exists in cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>True if exists</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Clear all cached values
    /// </summary>
    /// <returns>Task</returns>
    Task ClearAsync();

    /// <summary>
    /// Remove cached values by pattern
    /// </summary>
    /// <param name="pattern">Key pattern</param>
    /// <returns>Task</returns>
    Task RemoveByPatternAsync(string pattern);
}
