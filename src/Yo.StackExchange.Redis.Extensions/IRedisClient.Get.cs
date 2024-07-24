using System.Threading.Tasks;
using StackExchange.Redis;

namespace Yo.StackExchange.Redis.Extensions;

/// <summary>
/// Redis Client 
/// </summary>
public partial interface IRedisClient
{
    /// <summary>
    /// Get the value of the specified key
    /// <typeparam name="TResult"></typeparam>
    /// </summary>
    /// <param name="key">Key of the string</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>The value of the specified key</returns>
    /// <remarks>https://redis.io/commands/get</remarks>
    Task<TResult> GetTAsync<TResult>(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Get the value of the specified key
    /// </summary>
    /// <param name="key">String key</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>The value of the specified key</returns>
    /// <remarks>https://redis.io/commands/get</remarks>
    Task<RedisValue> GetAsync(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Get multiple values ​​of specified keys (array)
    /// </summary>
    /// <param name="keys">String key set</param>
    /// <returns>Specified key value set</returns>
    /// <remarks>https://redis.io/commands/mget</remarks>
    Task<RedisValue[]> MGetAsync(params RedisKey[] keys);

    /// <summary>
    /// Get multiple values ​​of the specified key (array)
    /// </summary>
    /// <param name="keys">Key set of strings</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Value set of the specified key</returns>
    /// <remarks>https://redis.io/commands/mget</remarks>
    Task<RedisValue[]> MGetAsync(RedisKey[] keys, CommandFlags flags);

    /// <summary>
    /// Get the value of the specified field stored in the hash table
    /// </summary>
    /// <param name="key">Hash table key</param>
    /// <param name="hashField">Hash table field</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Hash table key value</returns>
    /// <remarks>https://redis.io/commands/hget</remarks>
    Task<RedisValue> HGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Get the values ​​of multiple fields stored in the hash table
    /// </summary>
    /// <param name="key">Hash table key</param>
    /// <param name="hashFields">Hash table fields</param>
    /// <returns>Hash table key value</returns>
    /// <remarks>https://redis.io/commands/hmget</remarks>
    Task<RedisValue[]> HMGetAsync(RedisKey key, params RedisValue[] hashFields);

    /// <summary>
    /// Get the values ​​of multiple fields stored in a hash table
    /// </summary>
    /// <param name="key">Hash table key</param>
    /// <param name="hashFields">Hash table fields</param>
    /// <param name="flags">Flags used for this operation</param>
    /// <returns>Hash table key value</returns>
    /// <remarks>https://redis.io/commands/hmget</remarks>
    Task<RedisValue[]> HMGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags);

    /// <summary> 
    /// Get all fields and values in a hash table 
    /// </summary>
    /// <param name="key">Hash table key</param>
    /// <param name="flags">Flags used for this operation</param>、
    /// <returns>Hash table all field and value</returns> 
    /// <remarks>https://redis.io/commands/hgetall"</remarks>
    Task<HashEntry[]> HGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the set cardinality (number of elements) of the set stored at key.
    /// </summary>
    /// <param name="key">The key of the set.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>The cardinality (number of elements) of the set, or 0 if key does not exist.</returns>
    /// <remarks>https://redis.io/commands/scard"</remarks>
    Task<long> SCardAsync(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns whether <paramref name="value" /> is a member of the set stored at <paramref name="key" />.
    /// </summary>
    /// <param name="key">The key of the set.</param>
    /// <param name="value">The value to check for.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>
    /// <see langword="true" /> if the element is a member of the set.
    /// <see langword="false" /> if the element is not a member of the set, or if key does not exist.
    /// </returns>
    /// <remarks>https://redis.io/commands/sismember"</remarks>
    Task<bool> SIsMemberAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns all the members of the set value stored at key.
    /// </summary>
    /// <param name="key">The key of the set.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>All elements of the set.</returns>
    /// <remarks>https://redis.io/commands/smembers"</remarks>
    Task<RedisValue[]> SMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return an array of count distinct elements if count is positive.
    /// If called with a negative count the behavior changes and the command is allowed to return the same element multiple times.
    /// In this case the number of returned elements is the absolute value of the specified count.
    /// </summary>
    /// <param name="key">The key of the set.</param>
    /// <param name="count">The count of members to get.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>An array of elements, or an empty array when <paramref name="key" /> does not exist.</returns>
    /// <remarks>https://redis.io/commands/srandmember"</remarks>
    Task<RedisValue[]> SRandMemberAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the sorted set cardinality (number of elements) of the sorted set stored at key.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>The cardinality (number of elements) of the sorted set, or 0 if key does not exist.</returns>
    /// <remarks>https://redis.io/commands/zcard</remarks>
    Task<long> ZCardAsync(RedisKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the specified range of elements in the sorted set stored at key.
    /// By default, the elements are considered to be ordered from the lowest to the highest score.
    /// Lexicographical order is used for elements with equal score.
    /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on.
    /// They can also be negative numbers indicating offsets from the end of the sorted set, with -1 being the last element of the sorted set, -2 the penultimate element and so on.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The start index to get.</param>
    /// <param name="stop">The stop index to get.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>List of elements in the specified range.</returns>
    /// <remarks>https://redis.io/commands/zrange</remarks>
    Task<RedisValue[]> ZRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the specified range of elements in the sorted set stored at key.
    /// By default, the elements are considered to be ordered from the lowest to the highest score.
    /// Lexicographical order is used for elements with equal score.
    /// Start and stop are used to specify the min and max range for score values.
    /// Similar to other range methods the values are inclusive.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The minimum score to filter by.</param>
    /// <param name="stop">The maximum score to filter by.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>List of elements in the specified score range.</returns>
    /// <remarks>https://redis.io/commands/zrangebyscore</remarks>
    Task<RedisValue[]> ZRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the score of member in the sorted set at key.
    /// If member does not exist in the sorted set, or key does not exist, <see langword="null" /> is returned.
    /// </summary>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to get a score for.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <returns>The score of the member.</returns>
    /// <remarks><seealso href="https://redis.io/commands/zscore" /></remarks>
    Task<double?> ZScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None);
}
