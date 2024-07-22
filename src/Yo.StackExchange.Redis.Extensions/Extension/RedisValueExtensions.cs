using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Yo.StackExchange.Redis.Extensions.Extension;

/// <summary>
/// <see cref="RedisValue"/> Extension
/// </summary>
public static class RedisValueExtensions
{
    public static HashEntry[] ConvertToHashEntryArray(this KeyValuePair<RedisValue, RedisValue>[] fieldNameValuePairs)
        => fieldNameValuePairs?.Select(field => new HashEntry(field.Key, field.Value)).ToArray();

    public static List<string> ConvertToStringList(this RedisValue[] values) => values?.Select(v => v.ToString()).ToList();

    public static List<string> ConvertToStringList(this RedisKey[] keys) => keys?.Select(k => k.ToString()).ToList();

    public static IDictionary<string, string> ConvertToStringDic(this KeyValuePair<RedisValue, RedisValue>[] fieldNameValuePairs)
        => fieldNameValuePairs?.Select(r => new KeyValuePair<string, string>(r.Key, r.Value)).ToDictionaryExt(r => r.Key, r => r.Value);

    public static IDictionary<string, string> ConvertToStringDic(this KeyValuePair<RedisKey, RedisValue>[] fieldNameValuePairs)
        => fieldNameValuePairs?.Select(r => new KeyValuePair<string, string>(r.Key, r.Value)).ToDictionaryExt(r => r.Key, r => r.Value);
}