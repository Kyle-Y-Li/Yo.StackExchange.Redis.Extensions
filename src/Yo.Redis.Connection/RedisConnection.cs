using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Options;

namespace Yo.Redis.Connection;

public class RedisConnection : IRedisConnection, IDisposable
{
    private readonly SemaphoreSlim _connectionLock = new(initialCount: 1, maxCount: 1);
    private bool _disposedValue;

    private readonly int _defaultDatabase;
    private readonly ConfigurationOptions _connectionOptions;
    private volatile IConnectionMultiplexer _redisConnection;

    /// <summary>
    /// Initializes a new instance of <see cref="IRedisConnection"/>.
    /// </summary>
    /// <param name="optionsAccessor">The configuration options.</param>
    public RedisConnection(IOptions<RedisConnectionOptions> optionsAccessor)
    {
        var redisOptions = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(RedisConnectionOptions));
        var redisConnection = redisOptions.Configuration ?? throw new ArgumentNullException(nameof(RedisConnectionOptions));
        _connectionOptions = ConfigurationOptions.Parse(redisConnection);

        _defaultDatabase = redisOptions.DefaultDatabase ?? -1;
        if (_defaultDatabase > -1) _connectionOptions.DefaultDatabase = _defaultDatabase;
        _defaultDatabase = _connectionOptions.DefaultDatabase ?? -1;

        if (redisOptions.ReconnectRetryPolicy != null) _connectionOptions.ReconnectRetryPolicy = redisOptions.ReconnectRetryPolicy;
    }

    private void Connect()
    {
        if (_redisConnection != null) return;

        _connectionLock.Wait();
        try
        {
            if (_redisConnection == null)
            {
                _redisConnection = ConnectionMultiplexer.Connect(_connectionOptions);
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task ConnectAsync(CancellationToken token = default)
    {
        if (_redisConnection != null) return;

        await _connectionLock.WaitAsync(token).ConfigureAwait(false);
        try
        {
            if (_redisConnection == null)
            {
                _redisConnection = await ConnectionMultiplexer.ConnectAsync(_connectionOptions).ConfigureAwait(false);
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private int GetDatabaseIdOrDefault(int db = -1) => db > -1 ? db : _defaultDatabase;

    /// <summary>
    /// Obtain an interactive connection to a database inside redis
    /// </summary>
    /// <param name="db">The database ID to get.</param>
    /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.IDatabase" />.</param>
    public IDatabase GetDatabase(int db = -1, object asyncState = null)
    {
        Connect();
        db = GetDatabaseIdOrDefault(db);
        return _redisConnection.GetDatabase(db, asyncState);
    }

    /// <summary>
    /// Obtain an interactive connection to a database inside redis
    /// </summary>
    /// <param name="db">The database ID to get.</param>
    /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.IDatabase" />.</param>
    /// <param name="token">The <see cref="T:System.Threading.CancellationToken"/> token to observe.</param>
    public async Task<IDatabase> GetDatabaseAsync(int db = -1, object asyncState = null, CancellationToken token = default)
    {
        await ConnectAsync(token).ConfigureAwait(false);
        db = GetDatabaseIdOrDefault(db);
        return _redisConnection.GetDatabase(db, asyncState);
    }

    /// <summary>
    /// Allows creation of a group of operations that will be sent to the server as a single unit,
    /// but which may or may not be processed on the server contiguously.
    /// </summary>
    /// <param name="db">The database ID to get.</param>
    /// <param name="asyncState">The async object state to be passed into the created <see cref="T:StackExchange.Redis.IBatch" />.</param>
    /// <returns>The created batch.</returns>
    public IBatch GetBatch(int db = -1, object asyncState = null)
    {
        var database = GetDatabase(db, asyncState);
        return database?.CreateBatch(asyncState);
    }

    /// <summary>
    /// Allows creation of a group of operations that will be sent to the server as a single unit,
    /// but which may or may not be processed on the server contiguously.
    /// </summary>
    /// <param name="db">The database ID to get.</param>
    /// <param name="asyncState">The async object state to be passed into the created <see cref="T:StackExchange.Redis.IBatch" />.</param>
    /// <param name="token">The <see cref="T:System.Threading.CancellationToken"/> token to observe.</param>
    /// <returns>The created batch.</returns>
    public async Task<IBatch> GetBatchAsync(int db = -1, object asyncState = null, CancellationToken token = default)
    {
        var database = await GetDatabaseAsync(db, asyncState, token);
        return database?.CreateBatch(asyncState);
    }

    /// <summary>
    /// Obtain a pub/sub subscriber connection to the specified server
    /// </summary>
    /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.ISubscriber" />.</param>
    public ISubscriber GetSubscriber(object asyncState = null)
    {
        Connect();
        return _redisConnection.GetSubscriber(asyncState);
    }

    /// <summary>
    /// Obtain a pub/sub subscriber connection to the specified server
    /// </summary>
    /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.ISubscriber" />.</param>
    /// <param name="token">The <see cref="T:System.Threading.CancellationToken"/> token to observe.</param>
    public async Task<ISubscriber> GetSubscriberAsync(object asyncState = null, CancellationToken token = default)
    {
        await ConnectAsync(token).ConfigureAwait(false);
        return _redisConnection.GetSubscriber(asyncState);
    }

    /// <summary>Obtain a configuration API for an individual server</summary>
    /// <param name="host">The host to get a server for.</param>
    /// <param name="port">The specific port for <paramref name="host" /> to get a server for.</param>
    /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.IServer" />.</param>
    public IServer GetServer(string host, int port, object asyncState = null)
    {
        Connect();
        return _redisConnection.GetServer(host, port, asyncState);
    }

    /// <summary>Obtain a configuration API for an individual server</summary>
    /// <param name="host">The host to get a server for.</param>
    /// <param name="port">The specific port for <paramref name="host" /> to get a server for.</param>
    /// <param name="asyncState">The async state to pass to the created <see cref="T:StackExchange.Redis.IServer" />.</param>
    /// <param name="token">The <see cref="T:System.Threading.CancellationToken"/> token to observe.</param>
    public async Task<IServer> GetServerAsync(string host, int port, object asyncState = null, CancellationToken token = default)
    {
        await ConnectAsync(token).ConfigureAwait(false);
        return _redisConnection.GetServer(host, port, asyncState);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _redisConnection?.Close();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~RedisConnection()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
