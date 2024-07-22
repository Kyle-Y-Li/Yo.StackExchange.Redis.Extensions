using Yo.Redis.Connection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Yo.StackExchange.Redis.Extensions.Test;
public class RedisConnectionTest
{
    private readonly IServiceCollection _serviceCollection;
    private readonly IConfiguration _configuration;
    private readonly ServiceProvider _serviceProvider;
    public RedisConnectionTest()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        _serviceCollection = new ServiceCollection()
            .AddOptions()
            .Configure<RedisConnectionOptions>(_configuration.GetSection("RedisConnectionOptions"));

        _serviceProvider = _serviceCollection.BuildServiceProvider();

        var redisConnectionString = _serviceProvider.GetService<IOptions<RedisConnectionOptions>>()!.Value.Configuration;
        _serviceProvider = _serviceCollection.AddRedisConnection(option =>
        {
            option.Configuration = redisConnectionString;
        }).BuildServiceProvider();
    }


    [Fact]
    public async Task GetDatabaseTestAsync()
    {
        var redisConnection = _serviceProvider.GetService<IRedisConnection>()!;

        var database = await redisConnection.GetDatabaseAsync(1);

        Assert.NotNull(database);
    }

    [Fact]
    public async Task GetBatchTestAsync()
    {
        var redisConnection = _serviceProvider.GetService<IRedisConnection>()!;

        var dataBatch = await redisConnection.GetBatchAsync(1);

        Assert.NotNull(dataBatch);
    }

    [Fact]
    public async Task GetSubscriberTestAsync()
    {
        var redisConnection = _serviceProvider.GetService<IRedisConnection>()!;

        var dataSubscriber = await redisConnection.GetSubscriberAsync();

        Assert.NotNull(dataSubscriber);
    }

    [Fact]
    public async Task GetServerTestAsync()
    {
        var redisConnection = _serviceProvider.GetService<IRedisConnection>()!;

        var dataServer = await redisConnection.GetServerAsync("127.0.0.1", 6379);

        Assert.NotNull(dataServer);
    }
}