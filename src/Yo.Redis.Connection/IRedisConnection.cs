using System.Threading.Tasks;
using System.Threading;
using StackExchange.Redis;

namespace Yo.Redis.Connection
{
    public interface IRedisConnection
    {
        /// <summary>
        /// Obtain an interactive connection to a database inside redis
        /// </summary>
        /// <param name="db">The database ID to get.</param>
        /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.IDatabase" />.</param>
        IDatabase GetDatabase(int db = -1, object asyncState = null);

        /// <summary>
        /// Obtain an interactive connection to a database inside redis
        /// </summary>
        /// <param name="db">The database ID to get.</param>
        /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.IDatabase" />.</param>
        /// <param name="token">The <see cref="T:System.Threading.CancellationToken"/> token to observe.</param>
        Task<IDatabase> GetDatabaseAsync(int db = -1, object asyncState = null, CancellationToken token = default);

        /// <summary>
        /// Allows creation of a group of operations that will be sent to the server as a single unit,
        /// but which may or may not be processed on the server contiguously.
        /// </summary>
        /// <param name="db">The database ID to get.</param>
        /// <param name="asyncState">The async object state to be passed into the created <see cref="T:StackExchange.Redis.IBatch" />.</param>
        /// <returns>The created batch.</returns>
        IBatch GetBatch(int db = -1, object asyncState = null);

        /// <summary>
        /// Allows creation of a group of operations that will be sent to the server as a single unit,
        /// but which may or may not be processed on the server contiguously.
        /// </summary>
        /// <param name="db">The database ID to get.</param>
        /// <param name="asyncState">The async object state to be passed into the created <see cref="T:StackExchange.Redis.IBatch" />.</param>
        /// <param name="token">The <see cref="T:System.Threading.CancellationToken"/> token to observe.</param>
        /// <returns>The created batch.</returns>
        Task<IBatch> GetBatchAsync(int db = -1, object asyncState = null, CancellationToken token = default);

        /// <summary>
        /// Obtain a pub/sub subscriber connection to the specified server
        /// </summary>
        /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.ISubscriber" />.</param>
        ISubscriber GetSubscriber(object asyncState = null);

        /// <summary>
        /// Obtain a pub/sub subscriber connection to the specified server
        /// </summary>
        /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.ISubscriber" />.</param>
        /// <param name="token">The <see cref="T:System.Threading.CancellationToken"/> token to observe.</param>
        Task<ISubscriber> GetSubscriberAsync(object asyncState = null, CancellationToken token = default);

        /// <summary>Obtain a configuration API for an individual server</summary>
        /// <param name="host">The host to get a server for.</param>
        /// <param name="port">The specific port for <paramref name="host" /> to get a server for.</param>
        /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.IServer" />.</param>
        IServer GetServer(string host, int port, object asyncState = null);

        /// <summary>Obtain a configuration API for an individual server</summary>
        /// <param name="host">The host to get a server for.</param>
        /// <param name="port">The specific port for <paramref name="host" /> to get a server for.</param>
        /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.IServer" />.</param>
        /// <param name="token">The <see cref="T:System.Threading.CancellationToken"/> token to observe.</param>
        Task<IServer> GetServerAsync(string host, int port, object asyncState = null, CancellationToken token = default);
    }
}
