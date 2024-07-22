using Yo.Redis.Connection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;
using Xunit;

namespace Yo.StackExchange.Redis.Extensions.Test;

public class RedisDistributedCacheTest
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IConfiguration _configuration;
    private readonly ServiceProvider _serviceProvider;
    private readonly IDistributedCache _distributedCache;
    private readonly byte[] _value = Encoding.UTF8.GetBytes("V");

    public RedisDistributedCacheTest()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        _serviceCollection = new ServiceCollection()
            .AddOptions()
            .Configure<RedisConnectionOptions>(_configuration.GetSection("RedisConnectionOptions"));

        _serviceProvider = _serviceCollection.BuildServiceProvider();

        var redisConnectionString = _serviceProvider.GetService<IOptions<RedisConnectionOptions>>()!.Value.Configuration;
        _serviceProvider = _serviceCollection.AddDistributedRedisCache(option =>
        {
            option.Configuration = redisConnectionString;
        }).BuildServiceProvider();

        _distributedCache = _serviceProvider.GetService<IDistributedCache>()!;
    }

    [Fact]
    public void SetTest()
    {
        var key = Guid.NewGuid().ToString("N");
        _distributedCache.Set(key, _value);
        var value = _distributedCache.Get(key);
        Assert.Equal(_value, value);
    }

    [Fact]
    public void SetAbsoluteExpirationTest()
    {
        var key = Guid.NewGuid().ToString("N");
        _distributedCache.Set(key, _value, new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddMinutes(10)));
        var value = _distributedCache.Get(key);
        Assert.Equal(_value, value);
    }

    [Fact]
    public void SetSlidingExpirationTest()
    {
        var key = Guid.NewGuid().ToString("N");
        _distributedCache.Set(key, _value, new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(5)));
        _distributedCache.GetString(key);
        Thread.Sleep(2 * 1000);
        _distributedCache.GetString(key);
        Thread.Sleep(2 * 1000);
        _distributedCache.GetString(key);
        Thread.Sleep(2 * 1000);
        var value = _distributedCache.Get(key);
        Assert.Equal(_value, value);
    }

    [Fact]
    public void SetAbsoluteExpirationRelativeToNowTest()
    {
        var key = Guid.NewGuid().ToString("N");
        _distributedCache.Set(key, _value, new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));
        var value = _distributedCache.Get(key);
        Assert.Equal(_value, value);
    }

    [Fact]
    public async Task SetSlidingExpirationTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        await _distributedCache.SetAsync(key, _value, new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(60)));
        var value = await _distributedCache.GetAsync(key);
        Assert.Equal(_value, value);
    }

    [Fact]
    public async Task RemoveTestAsync()
    {
        var key = Guid.NewGuid().ToString("N");
        await _distributedCache.SetAsync(key, _value);
        await _distributedCache.RemoveAsync(key);
        var value = await _distributedCache.GetAsync(key);
        Assert.Null(value);
    }
}