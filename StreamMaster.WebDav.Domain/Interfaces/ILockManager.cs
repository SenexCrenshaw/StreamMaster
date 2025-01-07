namespace StreamMaster.WebDav.Domain.Interfaces;

/// <summary>
/// Interface for managing global locks for WebDAV resources.
/// </summary>
public interface ILockManager
{
    /// <summary>
    /// Acquires a lock for a resource.
    /// </summary>
    /// <param name="resourcePath">The path of the resource to lock.</param>
    /// <param name="owner">The owner of the lock.</param>
    /// <param name="isExclusive">Indicates whether the lock is exclusive.</param>
    /// <param name="timeout">The duration of the lock.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A lock token if successful; otherwise, null.</returns>
    Task<string?> AcquireLockAsync(string resourcePath, string owner, bool isExclusive, TimeSpan timeout, CancellationToken cancellationToken);

    /// <summary>
    /// Releases a lock for a resource.
    /// </summary>
    /// <param name="lockToken">The token representing the lock.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<bool> ReleaseLockAsync(string lockToken, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a resource is currently locked.
    /// </summary>
    /// <param name="resourcePath">The path of the resource to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the resource is locked; otherwise, false.</returns>
    Task<bool> IsLockedAsync(string resourcePath, CancellationToken cancellationToken);
}