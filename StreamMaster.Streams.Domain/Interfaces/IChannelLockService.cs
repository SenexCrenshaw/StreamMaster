namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Provides locking mechanism for channel-based operations to ensure thread safety.
/// </summary>
public interface IChannelLockService<T>
{
    /// <summary>
    /// Acquires the lock for the given channel ID, waiting if necessary.
    /// </summary>
    /// <param name="channelId">The channel ID for which to acquire the lock.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AcquireLockAsync(T channelId);

    /// <summary>
    /// Releases the lock for the given channel ID.
    /// </summary>
    /// <param name="channelId">The channel ID for which to release the lock.</param>
    void ReleaseLock(T channelId);

    /// <summary>
    /// Removes the lock for the given channel ID.
    /// </summary>
    /// <param name="channelId"></param>
    void RemoveLock(T channelId);
}
