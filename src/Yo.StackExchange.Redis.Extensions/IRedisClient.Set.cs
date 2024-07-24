using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Yo.StackExchange.Redis.Extensions.Entities;
using Yo.StackExchange.Redis.Extensions.Enum;

namespace Yo.StackExchange.Redis.Extensions;

/// <summary>
/// Redis Client 
/// </summary>
public partial interface IRedisClient
{
    /// <summary>Set the value of the specified key</summary>
    /// <param name="key">Key to be set</param>
    /// <param name="value">Value to be set</param>
    /// <param name="expiry">Expiration time</param>
    /// <param name="redisSetMode">Nx, Xx</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Set successfully, return true</returns>
    /// <remarks>https://redis.io/commands/set</remarks>
    Task<bool> SetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, RedisSetModeEnum redisSetMode = RedisSetModeEnum.None, CommandFlags flags = CommandFlags.None);

    /// <summary>Set the value only if key does not exist</summary>
    /// <param name="key">The key to be set</param>
    /// <param name="value">The value to be set</param>
    /// <param name="expiry">Expiration time</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>If key does not exist and the setting is successful, return true</returns>
    /// <remarks>https://redis.io/commands/setnx</remarks>
    Task<bool> SetNxAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None);

    /// <summary>Set the value of the specified key</summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="key">Key to set</param>
    /// <param name="value">Value to set</param>
    /// <param name="expiry">Expiration time</param>
    /// <param name="redisSetMode">Nx, Xx</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Set successfully, return true</returns>
    /// <remarks>https://redis.io/commands/set</remarks>
    Task<bool> SetTAsync<TSource>(RedisKey key, TSource value, TimeSpan? expiry = null, RedisSetModeEnum redisSetMode = RedisSetModeEnum.None, CommandFlags flags = CommandFlags.None);

    /// <summary>Set the value only if the key does not exist</summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="key">Key to set</param>
    /// <param name="value">Value to be set</param>
    /// <param name="expiry">Expiration time</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>If key does not exist and is set successfully, return true</returns>
    /// <remarks>https://redis.io/commands/setnx</remarks>
    Task<bool> SetNxTAsync<TSource>(RedisKey key, TSource value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// If the key already exists and is a string, the APPEND command appends the specified value to the end of the original value of the key
    /// </summary>
    /// <param name="key">The key of the string</param>
    /// <param name="value">The value to append to the string</param>
    /// <param name="flags">Flags for this operation</param>
    /// <returns>The length of the string after the append operation</returns>
    /// <remarks>https://redis.io/commands/append</remarks>
    Task<long> AppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>Used to delete a key if it exists</summary>
    /// <param name="keys">Keys to delete</param>
    /// <returns>Number of keys removed</returns>
    /// <remarks>https://redis.io/commands/del</remarks>
    Task<long> DelAsync(params RedisKey[] keys);

    /// <summary>Used to delete a key if it exists</summary>
    /// <param name="keys">Keys to delete</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Number of keys removed</returns>
    /// <remarks>https://redis.io/commands/del</remarks>
    Task<long> DelAsync(RedisKey[] keys, CommandFlags flags);

    /// <summary>Delete one or more hash table fields</summary>
    /// <param name="key">Hash table key to be deleted</param>
    /// <param name="fields">Hash table fields to be deleted</param>
    /// <returns>Number of fields removed</returns>
    /// <remarks>https://redis.io/commands/hdel</remarks>
    Task<long> HDelAsync(RedisKey key, params RedisValue[] fields);

    /// <summary>Delete one or more hash table fields</summary>
    /// <param name="key">Hash table key to delete</param>
    /// <param name="fields">Hash table fields to delete</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Number of fields removed</returns>
    /// <remarks>https://redis.io/commands/hdel</remarks>
    Task<long> HDelAsync(RedisKey key, RedisValue[] fields, CommandFlags flags);

    /// <summary>Set the value of the field field in the hash table key to value</summary>
    /// <param name="key">The hash table key to be set</param>
    /// <param name="field">The hash table field to be set</param>
    /// <param name="value">The hash table value to be set</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>If the field is a newly created field in the hash table and the value is set successfully, return true. If the field in the hash table already exists and the old value has been overwritten by the new value, return false. </returns>
    /// <remarks>https://redis.io/commands/hset</remarks>
    Task<bool> HSetAsync(RedisKey key, RedisValue field, RedisValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>Set the value of a hash table field only if the field field does not exist</summary>
    /// <param name="key">The hash table key to be set</param>
    /// <param name="field">The hash table field to be set</param>
    /// <param name="value">The hash table value to be set</param>
    /// <param name="flags">The flags used for this operation</param>
    /// <returns>Set successfully, return true</returns>
    /// <remarks>https://redis.io/commands/hsetnx</remarks>
    Task<bool> HSetNxAsync(RedisKey key, RedisValue field, RedisValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// <summary>Set multiple field-value pairs to the hash table key at the same time</summary>
    /// </summary>
    /// <param name="key">Hash table key to be set</param>
    /// <param name="fieldNameValuePairs">Hash table field name-value (domain-value) pairs to be set</param>
    /// <returns></returns>
    /// <remarks>https://redis.io/commands/hmset</remarks>
    /// <returns>Set successfully, return true</returns>
    Task<bool> HMSetAsync(RedisKey key, params KeyValuePair<RedisValue, RedisValue>[] fieldNameValuePairs);

    /// <summary>
    /// <summary>Set multiple field-value pairs to hash table key at the same time</summary>
    /// </summary>
    /// <param name="key">Hash table key to be set</param>
    /// <param name="fieldNameValuePairs">Hash table field name-value (domain-value) pairs to be set</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns></returns>
    /// <remarks>https://redis.io/commands/hmset</remarks>
    /// <returns>Set successfully, return true</returns>
    Task<bool> HMSetAsync(RedisKey key, KeyValuePair<RedisValue, RedisValue>[] fieldNameValuePairs, CommandFlags flags);

    /// <summary>Set one or more key-value pairs at the same time</summary>
    /// <param name="keyValuePairs">key-value pair</param>
    /// <returns>Set successfully, return true</returns>
    /// <remarks>https://redis.io/commands/mset</remarks>
    Task<bool> MSetAsync(params KeyValuePair<RedisKey, RedisValue>[] keyValuePairs);

    /// <summary>Set one or more key-value pairs at the same time</summary>
    /// <param name="keyValuePairs">key-value pairs</param>
    /// <param name="redisSetMode">Nx, Xx</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Set successfully, return true</returns>
    /// <remarks>https://redis.io/commands/mset</remarks>
    Task<bool> MSetAsync(KeyValuePair<RedisKey, RedisValue>[] keyValuePairs, RedisSetModeEnum redisSetMode, CommandFlags flags = CommandFlags.None);

    /// <summary>Set one or more key-value pairs at the same time, if and only if all given keys do not exist</summary>
    /// <param name="keyValuePairs">key-value pair</param>
    /// <returns>If key does not exist and setting is successful, return true</returns>
    /// <remarks>https://redis.io/commands/msetnx</remarks>
    Task<bool> MSetNxAsync(params KeyValuePair<RedisKey, RedisValue>[] keyValuePairs);

    /// <summary>Set one or more key-value pairs at the same time, if and only if all given keys do not exist</summary>
    /// <param name="keyValuePairs">key-value pair</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>If the key does not exist and the setting is successful, return true</returns>
    /// <remarks>https://redis.io/commands/msetnx</remarks>
    Task<bool> MSetNxAsync(KeyValuePair<RedisKey, RedisValue>[] keyValuePairs, CommandFlags flags);

    /// <summary>Modify the name of the key</summary>
    /// <param name="key">Old name</param>
    /// <param name="newKey">New name</param>
    /// <param name="redisSetMode">Nx, Xx</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Modification successful, return true</returns>
    /// <remarks>https://redis.io/commands/rename</remarks>
    Task<bool> RenameAsync(RedisKey key, RedisKey newKey, RedisSetModeEnum redisSetMode = RedisSetModeEnum.None, CommandFlags flags = CommandFlags.None);

    /// <summary>Modify the name of the key if and only if the given newKey does not exist</summary>
    /// <param name="key">Old name</param>
    /// <param name="newKey">New name</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>newKey does not exist and the modification is successful, return true</returns>
    /// <remarks>https://redis.io/commands/renamenx</remarks>
    Task<bool> RenameNxAsync(RedisKey key, RedisKey newKey, CommandFlags flags = CommandFlags.None);

    /// <summary>Set expiration time for a given key</summary>
    /// <param name="key">Key to set</param>
    /// <param name="expiry">Expiration time</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Set successfully, return true</returns>
    /// <remarks>https://redis.io/commands/expire</remarks>
    Task<bool> ExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None);

    /// <summary>Insert one or more values ​​into the head of the list</summary>
    /// <param name="key">Key to set</param>
    /// <param name="values">One ​​or more values</param>
    /// <returns>Length of the list after executing the LPUSH command</returns>
    /// <remarks>https://redis.io/commands/lpush</remarks>
    Task<long> LPushAsync(RedisKey key, params RedisValue[] values);

    /// <summary>Insert one or more values ​​to the head of a list</summary>
    /// <param name="key">The key to set</param>
    /// <param name="values">One ​​or more values</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>The length of the list after executing the LPUSH command</returns>
    /// <remarks>https://redis.io/commands/lpush</remarks>
    Task<long> LPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags);

    /// <summary>Insert a value to the head of a list</summary>
    /// <param name="key">The key to set</param>
    /// <param name="value">A value</param>
    /// <param name="redisSetMode">Nx, Xx</param>RPushXAsync
    /// <param name="flags">Flags for this operation</param>
    /// <returns>The length of the list after executing the LPUSH command</returns>
    /// <remarks>https://redis.io/commands/lpush</remarks>
    Task<long> LPushAsync(RedisKey key, RedisValue value, RedisSetModeEnum redisSetMode = RedisSetModeEnum.None, CommandFlags flags = CommandFlags.None);

    /// <summary>Insert a value into the head of an existing list</summary>
    /// <param name="key">List key to set</param>
    /// <param name="value">Value to set</param>
    /// <param name="flags">Flags for this operation</param>
    /// <returns>Execute LPUSH Command, the length of the list</returns>
    /// <remarks>https://redis.io/commands/lpushx</remarks>
    Task<long> LPushXAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>Add one or more values ​​to the list</summary>
    /// <param name="key">List key to set</param>
    /// <param name="values">One ​​or more values</param>
    /// <returns>After executing the RPUSH command, the length of the list</returns>
    /// <remarks>https://redis.io/commands/rpush</remarks>
    Task<long> RPushAsync(RedisKey key, params RedisValue[] values);

    /// <summary>Add one or more values ​​to the list</summary>
    /// <param name="key">List key to set</param>
    /// <param name="values">One ​​or more values</param>
    /// <param name="flags">Flags to use for this operation</param>
    /// <returns>Length of the list after executing the RPUSH command</returns>
    /// <remarks>https://redis.io/commands/rpush</remarks>
    Task<long> RPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags);

    /// <summary>Add a value to the list</summary>
    /// <param name="key">List key to set</param>
    /// <param name="value">Value to set</param>
    /// <param name="redisSetMode">Nx, Xx</param>
    /// <param name="flags">Flags to use for this operation</param>
    /// <returns>The length of the list after executing the RPUSH command</returns>
    /// <remarks>https://redis.io/commands/rpush</remarks>
    Task<long> RPushAsync(RedisKey key, RedisValue value, RedisSetModeEnum redisSetMode = RedisSetModeEnum.None, CommandFlags flags = CommandFlags.None);

    /// <summary>Add a value to an existing list</summary>
    /// <param name="key">List key to set</param>
    /// <param name="value">Value to set</param>
    /// <param name="flags">Flags for this operation</param>
    /// <returns>The length of the list after executing the RPUSHX command</returns>
    /// <remarks>https://redis.io/commands/rpushx</remarks>
    Task<long> RPushXAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>Remove and get the first element of the list</summary>
    /// <param name="key">List key to operate</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>The element removed</returns>
    /// <remarks>https://redis.io/commands/lpop</remarks>
    Task<RedisValue> LPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>Remove and get the last element of the list</summary>
    /// <param name="key">List key to operate</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>The element removed</returns>
    /// <remarks>https://redis.io/commands/rpop</remarks>
    Task<RedisValue> RPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Pop the last element (tail element) in the list sourceKey and return it to the client.
    /// Insert the element popped by sourceKey into the list destinationKey as the head element of the destinationKey list.
    /// </summary>
    /// <param name="sourceKey">Source key</param>
    /// <param name="destinationKey">Destination key</param>
    /// <param name="flags">Flags for this operation</param>
    /// <returns>The element to be moved</returns>
    /// <remarks>https://redis.io/commands/rpoplpush</remarks>
    Task<RedisValue> RPopLPushAsync(RedisKey sourceKey, RedisKey destinationKey, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Add one or more members to a collection
    /// </summary>
    /// <param name="key">The key of the collection</param>
    /// <param name="values">The values ​​to add to the collection</param>
    /// <returns>The number of elements added to the collection, excluding all elements that already exist in the collection</returns>
    /// <remarks>https://redis.io/commands/sadd</remarks>
    Task<long> SAddAsync(RedisKey key, params RedisValue[] values);

    /// <summary>
    /// Add one or more members to a set
    /// </summary>
    /// <param name="key">The key of the set</param>
    /// <param name="values">The values ​​to add to the set</param>
    /// <param name="flags">The flags to use for this operation</param>
    /// <returns>The number of elements added to the set, excluding any elements that already exist in the set</returns>
    /// <remarks>https://redis.io/commands/sadd</remarks>
    Task<long> SAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags);

    /// <summary>
    /// Removes and returns a random element from the set value stored at key.
    /// </summary>
    /// <param name="key">The key of the set.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>The removed element, or <see cref="P:StackExchange.Redis.RedisValue.Null" /> when key does not exist.</returns>
    /// <remarks>https://redis.io/commands/spop"</remarks>
    Task<RedisValue> SPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Adds all the specified members with the specified scores to the sorted set stored at key. It is possible to specify multiple score / member pairs. If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
    /// If key does not exist, a new sorted set with the specified members as sole members is created, like if the sorted set was empty.If the key exists but does not hold a sorted set, an error is returned.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">The members and values to add/update to the sorted set.</param>
    /// <returns>The number of elements changed.</returns>
    /// <remarks>https://redis.io/commands/zadd"</remarks>
    Task<long> ZAddAsync(RedisKey key, params SortedSetEntry[] values);

    /// <summary>
    /// Adds all the specified members with the specified scores to the sorted set stored at key. It is possible to specify multiple score / member pairs. If a specified member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
    /// If key does not exist, a new sorted set with the specified members as sole members is created, like if the sorted set was empty.If the key exists but does not hold a sorted set, an error is returned.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">The members and values to add/update to the sorted set.</param>
    /// <param name="when">What conditions to add the element under (defaults to always).</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>The number of elements changed.</returns>
    /// <remarks>https://redis.io/commands/zadd"</remarks>
    Task<long> ZAddAsync(RedisKey key, SortedSetEntry[] values, SortedSetWhen when, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Increments the score of member in the sorted set stored at key by increment. If member does not exist in the sorted set, it is added with increment as its score (as if its previous score was 0.0).
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to increment.</param>
    /// <param name="value">The amount to increment by.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>The new score of member.</returns>
    /// <remarks>https://redis.io/commands/zincrby</remarks>
    Task<double> ZIncrbyAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified members from the sorted set stored at key. Non-existing members are ignored.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to remove.</param>
    /// <returns>The number of members removed from the sorted set, not including Non-existing members.</returns>
    /// <remarks>https://redis.io/commands/zrem</remarks>
    Task<long> ZRemAsync(RedisKey key, params RedisValue[] members);

    /// <summary>
    /// Removes the specified members from the sorted set stored at key. Non-existing members are ignored.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to remove.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>The number of members removed from the sorted set, not including Non-existing members.</returns>
    /// <remarks>https://redis.io/commands/zrem</remarks>
    Task<long> ZRemAsync(RedisKey key, RedisValue[] members, CommandFlags flags);

    /// <summary>
    /// Removes all elements in the sorted set stored at key with rank between start and stop.
    /// Both start and stop are 0 -based indexes with 0 being the element with the lowest score.
    /// These indexes can be negative numbers, where they indicate offsets starting at the element with the highest score.
    /// For example: -1 is the element with the highest score, -2 the element with the second highest score and so forth.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The minimum rank to remove.</param>
    /// <param name="stop">The maximum rank to remove.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>The number of elements removed.</returns>
    /// <remarks>https://redis.io/commands/zremrangebyrank"</remarks>
    Task<long> ZRemRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None);

    /// <summary>Open distributed lock, return null if timeout</summary>
    /// <param name="key">Lock name</param>
    /// <param name="expiry">Timeout</param>
    /// <returns>Open successfully, return the lock. Open failed, return null</returns>
    Task<RedisLock> LockAsync(RedisKey key, TimeSpan expiry);

    /// <summary>
    /// After opening the distributed lock, execute the specified <paramref name="invokeFunc"/> method and release the distributed lock
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="key">Lock name</param>
    /// <param name="expiry">Timeout</param>
    /// <param name="invokeFunc">Specified method</param>
    /// <returns>Open successfully, return <paramref name="invokeFunc"/> return body. Open failed, return TResult default value</returns>
    Task<TResult> LockAsync<TResult>(RedisKey key, TimeSpan expiry, Func<Task<TResult>> invokeFunc);

    /// <summary>
    /// After opening the distributed lock, execute the specified <paramref name="invokeFunc"/> method and release the distributed lock
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="key">Lock name</param>
    /// <param name="expiry">Timeout</param>
    /// <param name="retryCount">Number of retries</param>
    /// <param name="invokeFunc">Specified method</param>
    /// <returns>Open successfully, return <paramref name="invokeFunc"/> return body. Open failed, return TResult default value</returns>
    Task<TResult> LockAsync<TResult>(RedisKey key, TimeSpan expiry, int retryCount, Func<Task<TResult>> invokeFunc);

    /// <summary>
    /// After opening the distributed lock, execute the specified <paramref name="invokeFunc"/> method and release the distributed lock
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="key">Lock name</param>
    /// <param name="expiry">Timeout</param>
    /// <param name="retryCount">Number of retries</param>
    /// <param name="retryInterval">Retry interval</param>
    /// <param name="invokeFunc">Specified method</param>
    /// <returns>Open successfully, return <paramref name="invokeFunc"/> return body. Open failed, return TResult default value</returns>
    Task<TResult> LockAsync<TResult>(RedisKey key, TimeSpan expiry, int retryCount, TimeSpan retryInterval, Func<Task<TResult>> invokeFunc);

    /// <summary>Add 1 to the value stored in key</summary>
    /// <param name="key">Key to be set</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns></returns>
    /// <remarks>https://redis.io/commands/incr</remarks>
    Task<long> IncrAsync(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>Add the value stored in key to the increment value given <paramref name="value"/></summary>
    /// <param name="key">Key to be set</param>
    /// <param name="value">Increment value</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns></returns>
    /// <remarks>https://redis.io/commands/incrby</remarks>
    Task<long> IncrByAsync(RedisKey key, long value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Execute Lua script
    /// </summary>
    /// <param name="script">Script text</param>
    /// <param name="redisKeys">Set key</param>
    /// <param name="redisValues">Set value</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Result of executing the script</returns>
    Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] redisKeys = null, RedisValue[] redisValues = null, CommandFlags flags = CommandFlags.None);
}