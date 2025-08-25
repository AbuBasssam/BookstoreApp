namespace Application.Interfaces;

public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key
    /// </summary>
    /// <typeparam name="T">Type of the cached object</typeparam>
    /// <param name="key">Cache key</param>
    /// <returns>Cached value or default if not found</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Gets a cached string value by key
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>Cached string value or null if not found</returns>
    Task<string?> GetStringAsync(string key);

    /// <summary>
    /// Sets a value in cache
    /// </summary>
    /// <typeparam name="T">Type of the object to cache</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="value">Value to cache</param>
    /// <param name="expiration">Expiration time (optional)</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Sets a string value in cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="value">String value to cache</param>
    /// <param name="expiration">Expiration time (optional)</param>
    Task SetStringAsync(string key, string value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a cached value by key
    /// </summary>
    /// <param name="key">Cache key</param>
    Task RemoveAsync(string key);

    /// <summary>
    /// Removes multiple cached values by keys
    /// </summary>
    /// <param name="keys">Cache keys</param>
    Task RemoveAsync(params string[] keys);

    /// <summary>
    /// Removes cached values by pattern
    /// </summary>
    /// <param name="pattern">Key pattern (supports wildcards)</param>
    Task RemoveByPatternAsync(string pattern);

    /// <summary>
    /// Checks if a key exists in cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>True if key exists, false otherwise</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Gets or sets a cached value
    /// </summary>
    /// <typeparam name="T">Type of the cached object</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="factory">Factory function to create value if not cached</param>
    /// <param name="expiration">Expiration time (optional)</param>
    /// <returns>Cached or newly created value</returns>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Sets cache expiration for an existing key
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="expiration">New expiration time</param>
    Task SetExpirationAsync(string key, TimeSpan expiration);

    /// <summary>
    /// Gets the time-to-live for a key
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>Time-to-live or null if key doesn't exist or has no expiration</returns>
    Task<TimeSpan?> GetTtlAsync(string key);
}