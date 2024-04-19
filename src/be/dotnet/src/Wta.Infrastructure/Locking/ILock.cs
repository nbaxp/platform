namespace Wta.Infrastructure.Locking;
public interface ILock
{
    Task<IDisposable?> Acquire(string key, TimeSpan timeout = default);
    Task<IDisposable?> TryAcquireAsync(string key, TimeSpan timeout = default);
}
