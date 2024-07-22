using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Yo.Redis.Connection;

namespace Yo.StackExchange.Redis.Extensions;

/// <summary>
/// Redis Client Implementation
/// </summary>
public partial class RedisClient : IRedisClient
{
    private readonly IRedisConnection _redisConnection;

    public RedisClient(IRedisConnection redisConnection)
    {
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
    }

    /// <summary>
    /// Get the value of the specified key
    /// </summary>
    /// <param name="key">String key</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>The value of the specified key</returns>
    /// <remarks>https://redis.io/commands/get</remarks>
    public async Task<RedisValue> GetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        return await _redisConnection.ExecuteAsync(redisClient => redisClient.StringGetAsync(key, flags));
    }

    /// <summary>
    /// Get multiple values ​​of specified keys (array)
    /// </summary>
    /// <param name="keys">String key set</param>
    /// <returns>Specified key value set</returns>
    /// <remarks>https://redis.io/commands/mget</remarks>
    public async Task<RedisValue[]> MGetAsync(params RedisKey[] keys)
    {
        return await MGetAsync(keys, CommandFlags.None);
    }

    /// <summary>
    /// Get multiple values ​​of the specified key (array)
    /// </summary>
    /// <param name="keys">Key set of strings</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Value set of the specified key</returns>
    /// <remarks>https://redis.io/commands/mget</remarks>
    public async Task<RedisValue[]> MGetAsync(RedisKey[] keys, CommandFlags flags)
    {
        return await _redisConnection.ExecuteAsync(redisClient => redisClient.StringGetAsync(keys, flags));
    }

    /// <summary>
    /// Get the value of the specified field stored in the hash table
    /// </summary>
    /// <param name="key">Hash table key</param>
    /// <param name="hashField">Hash table field</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Hash table key value</returns>
    /// <remarks>https://redis.io/commands/hget</remarks>
    public async Task<RedisValue> HGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
    {
        return await _redisConnection.ExecuteAsync(redisClient => redisClient.HashGetAsync(key, hashField, flags));
    }

    /// <summary>
    /// Get the values ​​of multiple fields stored in the hash table
    /// </summary>
    /// <param name="key">Hash table key</param>
    /// <param name="hashFields">Hash table fields</param>
    /// <returns>Hash table key value</returns>
    /// <remarks>https://redis.io/commands/hmget</remarks>
    public async Task<RedisValue[]> HMGetAsync(RedisKey key, params RedisValue[] hashFields)
    {
        return await HMGetAsync(key, hashFields, CommandFlags.None);
    }

    /// <summary>
    /// Get the values ​​of multiple fields stored in a hash table
    /// </summary>
    /// <param name="key">Hash table key</param>
    /// <param name="hashFields">Hash table fields</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Hash table key value</returns>
    /// <remarks>https://redis.io/commands/hmget</remarks>
    public async Task<RedisValue[]> HMGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags)
    {
        return await _redisConnection.ExecuteAsync(redisClient => redisClient.HashGetAsync(key, hashFields, flags));
    }

    /// <summary> 
    /// Get all fields and values in a hash table 
    /// </summary>
    /// <param name="key">Hash table key</param>
    /// <param name="flags">Flags used for this operation</param>、
    /// <returns>Hash table all field and value</returns> 
    /// <remarks><seealso href="https://redis.io/commands/hgetall" /></remarks>
    public async Task<HashEntry[]> HGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        return await _redisConnection.ExecuteAsync(redisClient => redisClient.HashGetAllAsync(key, flags));
    }

    /// <summary>
    /// Get the value of the specified key
    /// <typeparam name="TResult"></typeparam>
    /// </summary>
    /// <param name="key">Key of the string</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>The value of the specified key</returns>
    /// <remarks>https://redis.io/commands/get</remarks>
    public async Task<TResult> GetTAsync<TResult>(RedisKey key, CommandFlags flags = CommandFlags.None)
    {
        string value = await GetAsync(key, flags);
        if (value is TResult result) return result;
        return value == null ? default : JsonSerializer.Deserialize<TResult>(value);
    }
}