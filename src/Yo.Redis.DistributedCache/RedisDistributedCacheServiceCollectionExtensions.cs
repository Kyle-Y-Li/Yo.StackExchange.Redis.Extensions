using System;
using Microsoft.Extensions.Caching.Distributed;
using Yo.Redis.Connection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up  Redis distributed cache related services in an <see cref="IServiceCollection" />.
/// </summary>
public static class RedisDistributedCacheServiceCollectionExtensions
{
    /// <summary>
    /// Adds redis distributed caching services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="setupAction">An <see cref="Action{RedisConnectionOptions}"/> to configure the provided
    /// <see cref="RedisConnectionOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDistributedRedisCache(this IServiceCollection services, Action<RedisConnectionOptions> setupAction)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

        //1.register redis connection
        services.AddRedisConnection(setupAction);
        //2.register redis distributed caching
        services.Add(ServiceDescriptor.Singleton<IDistributedCache, RedisDistributedCache>());

        return services;
    }
}