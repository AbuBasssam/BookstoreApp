using Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace Infrastructure.Implementations;
/// <summary>
/// Redis implementation of ICacheService
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly JsonSerializerSettings _jsonSettings;

    public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
    {
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _database = _connectionMultiplexer.GetDatabase();

        _jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    /// <summary>
    /// Gets a cached value by key
    /// </summary>
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            var cachedValue = await _database.StringGetAsync(key);

            if (string.IsNullOrWhiteSpace(cachedValue))
                return null;

            return JsonConvert.DeserializeObject<T>(cachedValue, _jsonSettings);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting cached value for key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// Gets a cached string value by key
    /// </summary>
    public async Task<string?> GetStringAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            return await _distributedCache.GetStringAsync(key);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting cached string value for key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// Sets a value in cache
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;

            var serializedValue = JsonConvert.SerializeObject(value, _jsonSettings);
            // var options = CreateDistributedCacheOptions(expiration);

            await _database.StringSetAsync(key, serializedValue, expiration);

            //_logger.LogDebug("Cached value set for key: {Key}", key);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting cached value for key: {Key}", key);
        }
    }

    /// <summary>
    /// Sets a string value in cache
    /// </summary>
    public async Task SetStringAsync(string key, string value, TimeSpan? expiration = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;

            var options = CreateDistributedCacheOptions(expiration);
            await _distributedCache.SetStringAsync(key, value, options);

            //_logger.LogDebug("Cached string value set for key: {Key}", key);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting cached string value for key: {Key}", key);
        }
    }

    /// <summary>
    /// Removes a cached value by key
    /// </summary>
    public async Task RemoveAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            await _distributedCache.RemoveAsync(key);
            //_logger.LogDebug("Cached value removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error removing cached value for key: {Key}", key);
        }
    }

    /// <summary>
    /// Removes multiple cached values by keys
    /// </summary>
    public async Task RemoveAsync(params string[] keys)
    {
        try
        {
            if (keys == null || keys.Length == 0)
                return;

            var validKeys = keys.Where(k => !string.IsNullOrWhiteSpace(k)).ToArray();
            if (validKeys.Length == 0)
                return;

            var redisKeys = validKeys.Select(k => (RedisKey)k).ToArray();
            await _database.KeyDeleteAsync(redisKeys);

            //_logger.LogDebug("Cached values removed for {Count} keys", validKeys.Length);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error removing cached values for multiple keys");
        }
    }

    /// <summary>
    /// Removes cached values by pattern
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return;

            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern).ToArray();

            if (keys.Length > 0)
            {
                await _database.KeyDeleteAsync(keys);
                //_logger.LogDebug("Removed {Count} cached values matching pattern: {Pattern}", keys.Length, pattern);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error removing cached values by pattern: {Pattern}", pattern);
        }
    }

    /// <summary>
    /// Checks if a key exists in cache
    /// </summary>
    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error checking if key exists: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Gets or sets a cached value
    /// </summary>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
    {
        try
        {
            // Try to get from cache first
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
            {
                // _logger.LogDebug("Cache hit for key: {Key}", key);
                return cachedValue;
            }

            // If not in cache, create new value
            //_logger.LogDebug("Cache miss for key: {Key}, creating new value", key);
            var newValue = await factory();

            if (newValue != null)
            {
                await SetAsync(key, newValue, expiration);
            }

            return newValue;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in GetOrSetAsync for key: {Key}", key);
            // If cache fails, still try to return the factory result
            return await factory();
        }
    }

    /// <summary>
    /// Sets cache expiration for an existing key
    /// </summary>
    public async Task SetExpirationAsync(string key, TimeSpan expiration)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            await _database.KeyExpireAsync(key, expiration);
            //_logger.LogDebug("Set expiration for key: {Key} to {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting expiration for key: {Key}", key);
        }
    }

    /// <summary>
    /// Gets the time-to-live for a key
    /// </summary>
    public async Task<TimeSpan?> GetTtlAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            var ttl = await _database.KeyTimeToLiveAsync(key);
            return ttl;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting TTL for key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// Creates DistributedCacheEntryOptions with expiration settings
    /// </summary>
    private static DistributedCacheEntryOptions CreateDistributedCacheOptions(TimeSpan? expiration)
    {
        var options = new DistributedCacheEntryOptions();

        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }

        return options;
    }
}
