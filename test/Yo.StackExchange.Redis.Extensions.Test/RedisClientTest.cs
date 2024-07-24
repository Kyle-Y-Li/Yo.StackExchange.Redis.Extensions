using Yo.Redis.Connection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text;
using Yo.StackExchange.Redis.Extensions.Entities;
using Xunit;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Metrics;

namespace Yo.StackExchange.Redis.Extensions.Test;

public class RedisClientTest
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IConfiguration _configuration;
    private readonly ServiceProvider _serviceProvider;
    private readonly IRedisClient _redisClient;
    private const string Field = "F";
    private readonly byte[] _value = Encoding.UTF8.GetBytes("V");

    public RedisClientTest()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        _serviceCollection = new ServiceCollection()
            .AddOptions()
            .Configure<RedisConnectionOptions>(_configuration.GetSection("RedisConnectionOptions"));

        _serviceProvider = _serviceCollection.BuildServiceProvider();

        var redisConnectionString = _serviceProvider.GetService<IOptions<RedisConnectionOptions>>()!.Value.Configuration;
        _serviceProvider = _serviceCollection.AddRedisClient(option =>
        {
            option.Configuration = redisConnectionString;
        }).BuildServiceProvider();

        _redisClient = _serviceProvider.GetService<IRedisClient>()!;
    }

    [Fact]
    public async Task SetTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.SetAsync(key, _value));
    }

    [Fact]
    public async Task SetNxTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.SetNxAsync(key, _value, TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task MSetTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.MSetAsync(new KeyValuePair<RedisKey, RedisValue>(key, _value)));
    }

    [Fact]
    public async Task ExpireTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        await _redisClient.SetAsync(key, _value);
        Assert.True(await _redisClient.ExpireAsync(key, TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task HSetTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.HSetAsync(key, Field, _value));
    }

    [Fact]
    public async Task HMSetTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.HMSetAsync(key, new KeyValuePair<RedisValue, RedisValue>(Field, _value), new KeyValuePair<RedisValue, RedisValue>($"{Field}1", _value)));
    }

    [Fact]
    public async Task LPushTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.Equal(2, await _redisClient.LPushAsync(key, Field, _value));
    }

    [Fact]
    public async Task LockTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        RedisLock redisLock = await _redisClient.LockAsync(key, TimeSpan.FromMinutes(5));
        Assert.NotNull(redisLock);
        await Task.Delay(2 * 1000);
        Assert.True(await redisLock.UnlockAsync());
    }

    [Fact]
    public async Task LockExecTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        var result = await _redisClient.LockAsync<string>(key, TimeSpan.FromMinutes(5), async () =>
        {
            await Task.Delay(2 * 1000);
            return await Task.FromResult("true");
        });
        Assert.Equal("true", result);
    }

    [Fact]
    public async Task LockExecReTryTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        var result = await _redisClient.LockAsync<string>(key, TimeSpan.FromMinutes(5), 2, TimeSpan.FromSeconds(2), async () =>
        {
            await Task.Delay(2 * 1000);
            return await Task.FromResult("true");
        });
        Assert.Equal("true", result);
    }

    [Fact]
    public async Task IncrTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.IncrAsync(key) > 0);
    }

    [Fact]
    public async Task GetTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        await _redisClient.SetAsync(key, _value);
        Assert.Equal(await _redisClient.GetAsync(key), _value);
    }

    [Fact]
    public async Task GetTTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        await _redisClient.SetAsync(key, _value);
        Assert.Equal(await _redisClient.GetTAsync<string>(key), Encoding.UTF8.GetString(_value));
    }

    [Fact]
    public async Task SAddTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.SAddAsync(key, _value, $"{_value}1") > 0);
    }

    [Fact]
    public async Task SPopAsyncTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        await _redisClient.SPopAsync(key);
    }

    [Fact]
    public async Task SCardTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.SCardAsync(key) == 0);
    }

    [Fact]
    public async Task ZAddTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.ZAddAsync(key, new SortedSetEntry(_value, 1), new SortedSetEntry($"{_value}1", 2)) == 0);
    }

    [Fact]
    public async Task ZRemRangeByRankTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        Assert.True(await _redisClient.ZRemRangeByRankAsync(key, 0, 1) == 0);
    }
}