using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Yo.Redis.Connection;

/// <summary>
/// Configuration options for <see cref="IRedisConnection"/>.
/// </summary>
public class RedisConnectionOptions : IOptions<RedisConnectionOptions>
{
    /// <summary>
    /// Configure the connection string. For example: 127.0.0.1:6379,password={password},defaultDatabase=0,ssl=false,writeBuffer=10240,abortConnect=false
    /// </summary>
    public string Configuration { get; set; }

    /// <summary>
    /// Default database index(0 to databases - 1)
    /// </summary>
    public int? DefaultDatabase { get; set; }

    /// <summary>
    /// Instance name
    /// </summary>
    public string InstanceName { get; set; }

    /// <summary>
    /// The retry policy to be used for connection reconnects
    /// </summary>
    public IReconnectRetryPolicy ReconnectRetryPolicy { get; set; }

    RedisConnectionOptions IOptions<RedisConnectionOptions>.Value => this;
}