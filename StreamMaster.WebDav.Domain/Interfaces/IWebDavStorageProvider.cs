using StreamMaster.WebDav.Domain.Models;

namespace StreamMaster.WebDav.Domain.Interfaces;

/// <summary>
/// Interface for handling WebDAV storage operations, including virtual and cached files.
/// </summary>
public interface IWebDavStorageProvider
{
    /// <summary>
    /// Retrieves the content of a file as a stream.
    /// </summary>
    /// <param name="path">The virtual path of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream containing the file content, or null if not found.</returns>
    Task<Stream?> GetFileAsync(string path, CancellationToken cancellationToken);

    Task<string?> GetTSStreamUrlAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Lists all entries (files and folders) in a given directory.
    /// </summary>
    /// <param name="path">The virtual path of the directory.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of directory entries.</returns>
    IAsyncEnumerable<DirectoryEntry> ListDirectoryAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a file or directory exists at the given path.
    /// </summary>
    /// <param name="path">The virtual path to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the file or directory exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Saves a non-virtual file to the local cache.
    /// </summary>
    /// <param name="path">The virtual path of the file.</param>
    /// <param name="content">The content stream of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveToLocalCacheAsync(string path, Stream content, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a directory at the specified path.
    /// </summary>
    Task CreateDirectoryAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a file or directory at the specified path.
    /// </summary>
    Task DeleteAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Copies a file or directory to a new location.
    /// </summary>
    Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken);

    /// <summary>
    /// Moves a file or directory to a new location.
    /// </summary>
    Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken);
}