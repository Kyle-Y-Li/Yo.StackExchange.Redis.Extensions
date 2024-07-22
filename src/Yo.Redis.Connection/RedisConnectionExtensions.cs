using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yo.Redis.Connection;

/// <summary>
/// <see cref="IRedisConnection"/> Extensions
/// </summary>
public static class RedisConnectionExtensions
{
    /// <summary>
    /// Single execution
    /// </summary>
    /// <param name="redisConnection"><see cref="IRedisConnection"/>Install</param>
    /// <param name="executeFunc">Execution Function</param>
    /// <returns></returns>
    public static async Task ExecuteAsync(
        this IRedisConnection redisConnection,
        Func<IDatabaseAsync, Task> executeFunc)
    {
        await redisConnection.ExecuteAsync(executeFunc, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Single execution - Has a return value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="redisConnection"><see cref="IRedisConnection"/>Install</param>
    /// <param name="executeFunc">Execution Function</param>
    /// <returns></returns>
    public static async Task<T> ExecuteAsync<T>(
        this IRedisConnection redisConnection,
        Func<IDatabaseAsync, Task<T>> executeFunc)
    {
        return await redisConnection.ExecuteAsync(executeFunc, null).ConfigureAwait(false);
    }

    /// <summary>
    /// Execute Redis commands
    /// </summary>
    /// <param name="redisConnection"><see cref="IRedisConnection"/>Install</param>
    /// <param name="executeFunc">Execution Function</param>
    /// <param name="redisDatabaseFunc">Customize<see cref="IDatabase"/></param>
    /// <returns></returns>
    public static async Task ExecuteAsync(
        this IRedisConnection redisConnection,
        Func<IDatabaseAsync, Task> executeFunc,
        Func<IDatabase, IDatabaseAsync> redisDatabaseFunc)
    {
        if (executeFunc == null) return;

        IDatabase redisDatabase = await redisConnection.GetDatabaseAsync().ConfigureAwait(false);
        IDatabaseAsync redisDatabaseAsync = redisDatabaseFunc?.Invoke(redisDatabase) ?? redisDatabase;
        await executeFunc.Invoke(redisDatabaseAsync).ConfigureAwait(false);
    }

    /// <summary>
    /// Execute Redis commands - Has a return value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="redisConnection"><see cref="IRedisConnection"/>Install</param>
    /// <param name="executeFunc">Execution Function</param>
    /// <param name="redisDatabaseFunc">Customize<see cref="IDatabase"/></param>
    /// <returns></returns>
    public static async Task<T> ExecuteAsync<T>(
        this IRedisConnection redisConnection,
        Func<IDatabaseAsync, Task<T>> executeFunc,
        Func<IDatabase, IDatabaseAsync> redisDatabaseFunc)
    {
        if (executeFunc == null) return default;

        IDatabase redisDatabase = await redisConnection.GetDatabaseAsync().ConfigureAwait(false);
        IDatabaseAsync redisDatabaseAsync = redisDatabaseFunc?.Invoke(redisDatabase) ?? redisDatabase;
        return await executeFunc.Invoke(redisDatabaseAsync).ConfigureAwait(false);
    }

    /// <summary>
    /// Batch Execution
    /// </summary>
    /// <param name="redisConnection"><see cref="IRedisConnection"/>Install</param>
    /// <param name="executeFunctions">Execution Function</param>
    /// <remarks>
    /// The purpose of Batch interface operation is to ensure sequentiality, so it is not recommended unless it is particularly necessary.<para></para>
    /// </remarks>
    /// <returns></returns>
    public static async Task BatchExecuteAsync(
        this IRedisConnection redisConnection,
        params Func<IDatabaseAsync, Task>[] executeFunctions)
    {
        await redisConnection.BatchExecuteAsync(database => database.CreateBatch(), executeFunctions).ConfigureAwait(false);
    }

    /// <summary>
    /// 批量Execute Redis commands
    /// </summary>
    /// <param name="redisConnection"><see cref="IRedisConnection"/>Install</param>
    /// <param name="redisDatabaseFunc">Customize<see cref="IDatabase"/></param>
    /// <param name="executeFunctions">Execution Function</param>
    /// <remarks>
    /// The purpose of Batch interface operation is to ensure sequentiality, so it is not recommended unless it is particularly necessary.<para></para>
    /// </remarks>
    /// <returns></returns>
    public static async Task BatchExecuteAsync(
        this IRedisConnection redisConnection,
        Func<IDatabase, IDatabaseAsync> redisDatabaseFunc,
        params Func<IDatabaseAsync, Task>[] executeFunctions)
    {
        if (executeFunctions == null || executeFunctions.Length < 1) return;

        IDatabase redisDatabase = await redisConnection.GetDatabaseAsync().ConfigureAwait(false);
        IDatabaseAsync redisDatabaseAsync = redisDatabaseFunc?.Invoke(redisDatabase) ?? redisDatabase;

        var executeTaskList = executeFunctions.Select(executeFunc => executeFunc.Invoke(redisDatabaseAsync)).ToList();
        (redisDatabaseAsync as IBatch)?.Execute(); //Explicitly submitting a batch operation
        await Task.WhenAll(executeTaskList).ConfigureAwait(false);
    }

    /// <summary>
    /// Batch Execution - Has a return value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="redisConnection"><see cref="IRedisConnection"/>Install</param>
    /// <param name="executeFunctions">Execution Function</param>
    /// <remarks>
    /// The purpose of Batch interface operation is to ensure sequentiality, so it is not recommended unless it is particularly necessary.<para></para>
    /// </remarks>
    /// <returns></returns>
    public static async Task<List<T>> BatchExecuteAsync<T>(
        this IRedisConnection redisConnection,
        params Func<IDatabaseAsync, Task<T>>[] executeFunctions)
    {
        return await redisConnection.BatchExecuteAsync(database => database.CreateBatch(), executeFunctions).ConfigureAwait(false);
    }

    /// <summary>
    /// 批量Execute Redis commands - Has a return value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="redisConnection"><see cref="IRedisConnection"/>Install</param>
    /// <param name="redisDatabaseFunc">Customize<see cref="IDatabase"/></param>
    /// <param name="executeFunctions">Execution Function</param>
    /// <remarks>
    /// The purpose of Batch interface operation is to ensure sequentiality, so it is not recommended unless it is particularly necessary.<para></para>
    /// </remarks>
    /// <returns></returns>
    public static async Task<List<T>> BatchExecuteAsync<T>(
        this IRedisConnection redisConnection,
        Func<IDatabase, IDatabaseAsync> redisDatabaseFunc,
        params Func<IDatabaseAsync, Task<T>>[] executeFunctions)
    {
        if (executeFunctions == null || executeFunctions.Length < 1) return default;

        IDatabase redisDatabase = await redisConnection.GetDatabaseAsync().ConfigureAwait(false);
        IDatabaseAsync redisDatabaseAsync = redisDatabaseFunc?.Invoke(redisDatabase) ?? redisDatabase;

        var executeTaskList = executeFunctions.Select(executeFunc => executeFunc.Invoke(redisDatabaseAsync)).ToList();
        (redisDatabaseAsync as IBatch)?.Execute(); //Explicitly submitting a batch operation
        return (await Task.WhenAll(executeTaskList).ConfigureAwait(false)).ToList();
    }
}