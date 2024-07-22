using System;
using Yo.Redis.Connection;

namespace Microsoft.Extensions.DependencyInjection;

public static class RedisConnectionServiceCollectionExtensions
{
    /// <summary>
    /// Adds Redis connection services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="setupAction">An <see cref="Action{RedisConnectionOptions}"/> to configure the provided
    /// <see cref="RedisConnectionOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddRedisConnection(this IServiceCollection services, Action<RedisConnectionOptions> setupAction)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

        services.AddOptions();
        services.Configure(setupAction);
        services.Add(ServiceDescriptor.Singleton<IRedisConnection, RedisConnection>());

        return services;
    }
}