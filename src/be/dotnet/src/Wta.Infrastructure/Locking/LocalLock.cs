namespace Wta.Infrastructure.Locking;

[Service<ILock>(ServiceLifetime.Singleton)]
public class LocalLock : ILock, IDisposable
{
    private readonly Dictionary<string, SemaphoreSlim> _semaphores = [];
    private bool disposedValue;

    public Task<IDisposable?> Acquire(string key, TimeSpan timeout = default)
    {
        var semaphore = GetOrCreateSemaphore(key);
        if (semaphore.Wait((int)timeout.TotalMilliseconds))
        {
            return Task.FromResult<IDisposable?>(new Release(semaphore));
        }
        return Task.FromResult<IDisposable?>(null);
    }

    public Task<IDisposable?> TryAcquireAsync(string key, TimeSpan timeout = default)
    {
        var semaphore = GetOrCreateSemaphore(key);
        if (semaphore.CurrentCount > 0)
        {
            return Task.FromResult<IDisposable?>(null);
        }
        return Acquire(key, timeout);
    }

    private SemaphoreSlim GetOrCreateSemaphore(string key)
    {
        lock (_semaphores)
        {
            if (!_semaphores.TryGetValue(key, out var semaphore))
            {
                semaphore = new SemaphoreSlim(1);
                _semaphores[key] = semaphore;
            }
            return semaphore;
        }
    }

    private class Release(SemaphoreSlim semaphoreSlim) : IDisposable
    {
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    semaphoreSlim.Release();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _semaphores.ForEach(o => o.Value.Dispose());
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
