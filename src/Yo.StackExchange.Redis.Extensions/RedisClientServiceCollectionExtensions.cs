using System;
using System.Collections.Generic;
using System.Linq;
using Yo.Redis.Connection;
using Yo.StackExchange.Redis.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class RedisClientServiceCollectionExtensions
{
    /// <summary>
    /// Register Redis Client
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/></param>
    /// <param name="setupAction">Custom <see cref="RedisConnectionOptions"/></param>
    /// <param name="implementationInstance">The <see cref="IRedisClient"/> instance of the service </param>
    /// <param name="implementationFactory">The <see cref="IRedisClient"/> factory that creates the service</param>
    public static IServiceCollection AddRedisClient(
        this IServiceCollection services,
        Action<RedisConnectionOptions> setupAction,
        Func<IEnumerable<IRedisClient>> implementationInstance = null,
        IEnumerable<Func<IServiceProvider, Func<IRedisClient>>> implementationFactory = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

        //1.register redis connection
        services.AddRedisConnection(setupAction);

        //2.register redis distributed caching
        services.AddDistributedRedisCache(setupAction);

        //3.register default redis client
        services.Add(ServiceDescriptor.Singleton<IRedisClient, RedisClient>());

        //4.register custom redis client instance
        implementationInstance?.Invoke().ToList().ForEach(instance => services.AddSingleton(instance));

        //5.register custom redis client factory
        implementationFactory?.ToList().ForEach(implement => services.AddSingleton(implement));

        return services;
    }
}