Yo.StackExchange.Redis.Extensions
===================

**Yo.StackExchange.Redis.Extensions** is a library that extends [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) allowing you a set of functionality needed by common applications. The library is signed and completely compatible with the **.netcoreapp3.1, .NET Standard 2.1, .NET 5.0, .NET 6.0, .NET 7.0, .NET 8.0**

Latest release is available on NuGet.

| Channel                  | Status                                                                                                                                                                                           |
| ------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Nuget(Yo.Redis.Connection)            | [![NuGet Status](http://img.shields.io/nuget/v/Yo.Redis.Connection.svg?style=flat)](https://www.nuget.org/packages/Yo.Redis.Connection/)
| Nuget(Yo.Redis.DistributedCache)            | [![NuGet Status](http://img.shields.io/nuget/v/Yo.Redis.DistributedCache.svg?style=flat)](https://www.nuget.org/packages/Yo.Redis.DistributedCache/)
| Nuget(Yo.StackExchange.Redis.Extensions)            | [![NuGet Status](http://img.shields.io/nuget/v/Yo.StackExchange.Redis.Extensions.svg?style=flat)](https://www.nuget.org/packages/Yo.StackExchange.Redis.Extensions/)


### 🚀 Quick start

```csharp
string redisConnectionString = "127.0.0.1:6379,password={password},defaultDatabase=0,ssl=false,writeBuffer=10240,abortConnect=false";
IServiceCollection serviceCollection = new ServiceCollection();
ServiceProvider serviceProvider = _serviceCollection.AddRedisClient(option =>
{
    option.Configuration = redisConnectionString;
}).BuildServiceProvider();

IRedisClient redisClient = _serviceProvider.GetService<IRedisClient>()!;

//get
var value = await redisClient.GetAsync("key");
string value = await redisClient.GetTAsync<string>("key");
var value = await _redisClient.MGetAsync("key1", "key2");

//set
await redisClient.SetAsync("key", "value");
await redisClient.SetNxAsync("key", "value");
await redisClient.MSetAsync(new KeyValuePair<RedisKey, RedisValue>("key1", "value1"), new KeyValuePair<RedisKey, RedisValue>("key2", "value2"));

//del
await _redisClient.DelAsync("key1", "key2");

//hash
var value = await _redisClient.HGetAsync("key1", "field1");
var value = await _redisClient.HMGetAsync("key", "field1", "field2");
var value = await _redisClient.HGetAllAsync("key");

await _redisClient.HSetAsync("key", "field", "value");
await _redisClient.HSetNxAsync("key", "field", "value");
await _redisClient.HMSetAsync("key", new KeyValuePair<RedisValue, RedisValue>("field1", "value1"), new KeyValuePair<RedisValue, RedisValue>("field2", "value2");

await _redisClient.HDelAsync("key", "field1", "field2");

//list
await _redisClient.LPushAsync("key", "value1", "value2");
await _redisClient.LPushXAsync("key", "value");

await _redisClient.RPushAsync("key", "value1", "value2");
await _redisClient.RPushXAsync("key", "value");

await _redisClient.LPopAsync("key");
await _redisClient.RPopAsync("key");

await _redisClient.RPopLPushAsync("sourceKey", "destinationKey");

//lock
var result = await redisClient.LockAsync<string>(key, TimeSpan.FromMinutes(5), async () =>
{
    await Task.Delay(2 * 1000);
    return await Task.FromResult("true");
});

var result = await _redisClient.LockAsync<string>(key, TimeSpan.FromMinutes(5), 2, TimeSpan.FromSeconds(2), async () =>
{
    await Task.Delay(2 * 1000);
    return await Task.FromResult("true");
});

//Lua script
private const string SetScript = (@"
    redis.call('HSET', KEYS[1], 'absexp', ARGV[1], 'sldexp', ARGV[2], 'data', ARGV[4])
    if ARGV[3] ~= '-1' then
      redis.call('EXPIRE', KEYS[1], ARGV[3])
    end
    return 1");
await redisClient.ScriptEvaluateAsync(SetScript, new RedisKey[] { "key1" },
    new RedisValue[]
    {
            "value1",
            "value2",
            value3,
            "value4"
    });

//other
await _redisClient.IncrAsync("key");
await _redisClient.DelAsync("key1", "key2");
await _redisClient.ExpireAsync("key1", TimeSpan.FromSeconds(5));

```
### 💻 RedisConnection

```csharp
IRedisConnection redisConnection = _serviceProvider.GetService<IRedisConnection>()!;

//StackExchange.Redis native functions
await _redisConnection.ExecuteAsync(redisClient => redisClient.StringIncrementAsync("key1", 1));
```

### 📰 DistributedCache

```csharp
IDistributedCache distributedCache = _serviceProvider.GetService<IDistributedCache>()!;

await distributedCache.SetAsync("key", Encoding.UTF8.GetBytes("value"));
var value = await distributedCache.GetAsync(key);

await distributedCache.SetAsync("key", Encoding.UTF8.GetBytes("value"), new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddMinutes(10)));
var value = await distributedCache.GetAsync(key);

await distributedCache.SetAsync("key", Encoding.UTF8.GetBytes("value"), new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));
var value = await distributedCache.GetAsync(key);

await distributedCache.SetAsync("key", Encoding.UTF8.GetBytes("value"), new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(5)));
var value = await distributedCache.GetAsync(key);

await _distributedCache.RemoveAsync("key");
```

### Q&A

For questions or issues do not hesitate to [raise an issue](https://github.com/Kyle-Y-Li/Yo.StackExchange.Redis.Extensions/issues/new/choose).

### Contributing

Thanks to all the people who already contributed!

<a href="https://github.com/Kyle-Y-Li/Yo.StackExchange.Redis.Extensions/graphs/contributors">
  <img src="https://contributors-img.web.app/image?repo=Kyle-Y-Li/Yo.StackExchange.Redis.Extensions" />
</a>

### License

StackExchange.Redis is Copyright © [StackExchange](https://github.com/StackExchange) and other contributors under the MIT license.

