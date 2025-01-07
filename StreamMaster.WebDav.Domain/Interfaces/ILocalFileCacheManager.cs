namespace StreamMaster.WebDav.Domain.Interfaces;

/// <summary>
/// Interface for managing local file caching operations.
/// </summary>
public interface ILocalFileCacheManager
{
    /// <summary>
    /// Saves a file to the local cache.
    /// </summary>
    /// <param name="path">The virtual path of the file.</param>
    /// <param name="content">The content stream to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveToCacheAsync(string path, Stream content, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a file from the local cache.
    /// </summary>
    /// <param name="path">The virtual path of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stream of the cached file, or null if not found.</returns>
    Task<Stream?> GetFromCacheAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a file exists in the local cache.
    /// </summary>
    /// <param name="path">The virtual path of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the file exists; otherwise, false.</returns>
    Task<bool> ExistsInCacheAsync(string path, CancellationToken cancellationToken);
}