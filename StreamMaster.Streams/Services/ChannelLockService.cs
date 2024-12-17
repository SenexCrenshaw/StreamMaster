using System.Collections.Concurrent;

namespace StreamMaster.Streams.Services;

public class ChannelLockService : IChannelLockService
{
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _locks = new();

    /// <inheritdoc />
    public async Task AcquireLockAsync(int channelId)
    {
        SemaphoreSlim semaphore = _locks.GetOrAdd(channelId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void ReleaseLock(int channelId)
    {
        if (_locks.TryGetValue(channelId, out SemaphoreSlim? semaphore))
        {
            semaphore.Release();
        }
    }
}
