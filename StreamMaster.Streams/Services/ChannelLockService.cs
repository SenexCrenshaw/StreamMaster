using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services;

/// <summary>
/// Service for managing locks on a per-channel basis.
/// </summary>
public class ChannelLockService : IChannelLockService, IDisposable
{
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _locks = new();
    private bool _disposed;

    /// <inheritdoc />
    public async Task AcquireLockAsync(int channelId)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(ChannelLockService));

        SemaphoreSlim semaphore = _locks.GetOrAdd(channelId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void ReleaseLock(int channelId)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(ChannelLockService));

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
    public void RemoveLock(int channelId)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(ChannelLockService));
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
