using System;
using System.Threading.Tasks;
using Yo.Redis.Connection;

namespace Yo.StackExchange.Redis.Extensions.Entities;

/// <summary>
/// Redis distributed lock
/// </summary>
public sealed class RedisLock : IAsyncDisposable
{
    private readonly IRedisConnection _redisConnection;
    private readonly string _key;
    private readonly string _value;
    private readonly TimeSpan _expiry;

    internal RedisLock(IRedisConnection redisConnection, string key, string value, TimeSpan expiry)
    {
        _redisConnection = redisConnection;
        _key = key;
        _value = value;
        _expiry = expiry;
    }

    /// <summary>Release distributed lock</summary>
    /// <returns>success/failure</returns>
    public async Task<bool> UnlockAsync()
    {
        return await _redisConnection.ExecuteAsync(redisClient => redisClient.LockReleaseAsync(_key, _value));
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await UnlockAsync();
    }
}