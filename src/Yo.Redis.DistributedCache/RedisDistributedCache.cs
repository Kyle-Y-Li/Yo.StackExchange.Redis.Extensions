using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using System.Threading;
using Yo.Redis.Connection;
using System.Linq;

namespace Microsoft.Extensions.Caching.Distributed;

public class RedisDistributedCache : IDistributedCache
{
    private const string SetScript = (@"
                redis.call('HSET', KEYS[1], 'absexp', ARGV[1], 'sldexp', ARGV[2], 'data', ARGV[4])
                if ARGV[3] ~= '-1' then
                  redis.call('EXPIRE', KEYS[1], ARGV[3])
                end
                return 1");
    private const string AbsoluteExpirationKey = "absexp";
    private const string SlidingExpirationKey = "sldexp";
    private const string DataKey = "data";
    private const long NotPresent = -1;

    private readonly IRedisConnection _redisConnection;
    private readonly string _redisInstance;

    private readonly Func<DistributedCacheEntryOptions, (long, long, long)> _getCacheExpirationOptions = options =>
    {
        var dateTimeNow = DateTimeOffset.UtcNow;
        if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= dateTimeNow)
            throw new ArgumentOutOfRangeException(nameof(DistributedCacheEntryOptions.AbsoluteExpiration), options.AbsoluteExpiration.Value, "The absolute expiration value must be in the future.");

        DateTimeOffset? absoluteExpiration = options.AbsoluteExpiration;
        if (options.AbsoluteExpirationRelativeToNow.HasValue) absoluteExpiration = dateTimeNow + options.AbsoluteExpirationRelativeToNow;
        TimeSpan? slidingExpiration = options.SlidingExpiration;

        long absoluteExpirationTicks = NotPresent;
        long absoluteExpirationSeconds = NotPresent;
        long slidingExpirationTicks = NotPresent;
        long slidingExpirationSeconds = NotPresent;
        long realExpirationSeconds = NotPresent;
        if (absoluteExpiration.HasValue)
        {
            absoluteExpirationTicks = absoluteExpiration.Value.Ticks;
            absoluteExpirationSeconds = (long)(absoluteExpiration.Value - dateTimeNow).TotalSeconds;

            realExpirationSeconds = absoluteExpirationSeconds;
        }
        if (slidingExpiration.HasValue)
        {
            slidingExpirationTicks = slidingExpiration.Value.Ticks;
            slidingExpirationSeconds = (long)slidingExpiration.Value.TotalSeconds;

            realExpirationSeconds = slidingExpirationSeconds;
        }
        if (absoluteExpirationSeconds > NotPresent && slidingExpirationSeconds > NotPresent)
        {
            realExpirationSeconds = Math.Min(absoluteExpirationSeconds, slidingExpirationSeconds);
        }

        return (absoluteExpirationTicks, slidingExpirationTicks, realExpirationSeconds);
    };

    private readonly Func<RedisValue[], (byte[], TimeSpan)> _getRefreshExpiration = redisValues =>
    {
        if (redisValues.All(v => v.IsNullOrEmpty)) return (null, TimeSpan.MinValue);

        long absoluteExpirationTicks = (long?)redisValues[0] ?? NotPresent;
        long slidingExpirationTicks = (redisValues.Length > 1 ? (long?)redisValues[1] : NotPresent) ?? NotPresent;
        var value = redisValues.Length > 2 ? (byte[])redisValues[2] : null;

        TimeSpan absoluteExpiration = TimeSpan.MinValue;
        if (absoluteExpirationTicks > -1) absoluteExpiration = new DateTimeOffset(absoluteExpirationTicks, TimeSpan.Zero) - DateTimeOffset.Now;
        TimeSpan slidingExpiration = TimeSpan.MinValue;
        if (slidingExpirationTicks > -1) slidingExpiration = new TimeSpan(slidingExpirationTicks);

        if (TimeSpan.MinValue.Equals(slidingExpiration)) return (value, TimeSpan.MinValue);

        return (value, absoluteExpiration > slidingExpiration ? absoluteExpiration : slidingExpiration);
    };

    /// <summary>
    /// Initializes a new instance of <see cref="RedisDistributedCache"/>.
    /// </summary>
    /// <param name="redisConnection">a new instance of <see cref="IRedisConnection"/>.</param>
    /// <param name="optionsAccessor">The configuration options.</param>
    public RedisDistributedCache(IRedisConnection redisConnection, IOptions<RedisConnectionOptions> optionsAccessor)
    {
        _redisConnection = redisConnection;
        _redisInstance = optionsAccessor?.Value?.InstanceName ?? string.Empty;
    }

    /// <summary>Gets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns>The located value or null.</returns>
    public byte[] Get(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        return GetAndRefresh(key);
    }

    /// <summary>Gets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the located value or null.</returns>
    public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        return await GetAndRefreshAsync(key, token);
    }

    /// <summary>Sets a value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="options">The cache options for the value.</param>
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (options == null) throw new ArgumentNullException(nameof(options));

        key = $"{_redisInstance}{key}";
        var redisDatabase = _redisConnection.GetDatabase();
        var (absoluteExpirationTicks, slidingExpirationTicks, expirationSeconds) = _getCacheExpirationOptions(options);
        redisDatabase.ScriptEvaluate(SetScript, new RedisKey[] { key },
            new RedisValue[]
            {
                    absoluteExpirationTicks,
                    slidingExpirationTicks,
                    expirationSeconds,
                    value
            });
    }

    /// <summary>Sets the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="options">The cache options for the value.</param>
    /// <param name="token">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (options == null) throw new ArgumentNullException(nameof(options));

        key = $"{_redisInstance}{key}";
        var redisDatabase = await _redisConnection.GetDatabaseAsync(0, null, token).ConfigureAwait(false);
        var (absoluteExpirationTicks, slidingExpirationTicks, expirationSeconds) = _getCacheExpirationOptions(options);
        await redisDatabase.ScriptEvaluateAsync(SetScript, new RedisKey[] { key },
            new RedisValue[]
            {
                    absoluteExpirationTicks,
                    slidingExpirationTicks,
                    expirationSeconds,
                    value
            }).ConfigureAwait(false);
    }

    /// <summary>Refreshes a value in the cache based on its key, resetting its sliding expiration timeout (if any).</summary>
    /// <param name="key">A string identifying the requested value.</param>
    public void Refresh(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        GetAndRefresh(key);
    }

    /// <summary>Gets a value with the given key and then refresh the value in the cache based on its key, resetting its sliding expiration timeout (if any)</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns>The located value or null.</returns>
    public byte[] GetAndRefresh(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        key = $"{_redisInstance}{key}";
        var redisDatabase = _redisConnection.GetDatabase();

        var values = redisDatabase.HashGet(key, new RedisValue[] { AbsoluteExpirationKey, SlidingExpirationKey, DataKey });
        if (values.All(v => v.IsNullOrEmpty)) return null;

        var (value, expirationSeconds) = _getRefreshExpiration(values);
        if (expirationSeconds > TimeSpan.MinValue)
        {
            redisDatabase.KeyExpire(key, expirationSeconds);
        }

        return value;
    }

    /// <summary>Refreshes a value in the cache based on its key, resetting its sliding expiration timeout (if any).</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        await GetAndRefreshAsync(key, token).ConfigureAwait(false);
    }

    /// <summary>Gets a value with the given key and then refresh the value in the cache based on its key, resetting its sliding expiration timeout (if any)</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The located value or null.</returns>
    public async Task<byte[]> GetAndRefreshAsync(string key, CancellationToken token = default)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        key = $"{_redisInstance}{key}";
        var redisDatabase = await _redisConnection.GetDatabaseAsync(0, null, token).ConfigureAwait(false);

        var values = await redisDatabase.HashGetAsync(key, new RedisValue[] { AbsoluteExpirationKey, SlidingExpirationKey, DataKey });
        if (values.All(v => v.IsNullOrEmpty)) return null;

        var (value, expirationSeconds) = _getRefreshExpiration(values);
        if (expirationSeconds > TimeSpan.MinValue)
        {
            await redisDatabase.KeyExpireAsync(key, expirationSeconds).ConfigureAwait(false);
        }

        return value;
    }

    /// <summary>Removes the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    public void Remove(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        key = $"{_redisInstance}{key}";
        var redisDatabase = _redisConnection.GetDatabase();

        redisDatabase.KeyDelete(key);
    }

    /// <summary>Removes the value with the given key.</summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">Optional. The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        key = $"{_redisInstance}{key}";
        var redisDatabase = await _redisConnection.GetDatabaseAsync(0, null, token).ConfigureAwait(false);

        await redisDatabase.KeyDeleteAsync(key).ConfigureAwait(false);
    }
}
