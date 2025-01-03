using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services;

/// <summary>
/// Service for managing locks on a per-channel basis.
/// </summary>
public class ChannelLockService<T> : IChannelLockService<T>, IDisposable where T : notnull
{
    private readonly ConcurrentDictionary<T, SemaphoreSlim> _locks = new();
    private bool _disposed;

    /// <inheritdoc />
    public async Task AcquireLockAsync(T channelId)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(ChannelLockService<T>));

        SemaphoreSlim semaphore = _locks.GetOrAdd(channelId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void ReleaseLock(T channelId)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(ChannelLockService<T>));

        if (_locks.TryGetValue(channelId, out SemaphoreSlim? semaphore))
        {
            try
            {
                semaphore.Release();
            }
            catch (SemaphoreFullException)
            {
                // Log warning if needed
            }
        }
    }

    /// <inheritdoc />
    public void RemoveLock(T channelId)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(ChannelLockService<T>));
        ReleaseLock(channelId);
        _locks.TryRemove(channelId, out _);
    }

    /// <summary>
    /// Disposes all semaphores in the service.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (SemaphoreSlim semaphore in _locks.Values)
        {
            semaphore.Dispose();
        }

        _locks.Clear();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
