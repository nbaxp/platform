using Medallion.Threading.Redis;
using StackExchange.Redis;

namespace Wta.Infrastructure.Locking;

public class DistributedLock : ILock, IDisposable
{
    private bool disposedValue;

    public DistributedLock(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis") ?? "127.0.0.1:6379";
        Database = ConnectionMultiplexer.ConnectAsync(connectionString).GetAwaiter().GetResult().GetDatabase();
    }

    public IDatabase Database { get; }

    public async Task<IDisposable?> Acquire(string key, TimeSpan timeout = default)
    {
        var @lock = new RedisDistributedLock(key, Database);
        return await @lock.AcquireAsync(timeout).ConfigureAwait(false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task<IDisposable?> TryAcquireAsync(string key, TimeSpan timeout = default)
    {
        var @lock = new RedisDistributedLock(key, Database);
        return await @lock.TryAcquireAsync(timeout).ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Database.Multiplexer.Dispose();
            }
            disposedValue = true;
        }
    }
}
