using StreamMaster.WebDav.Domain.Models;

namespace StreamMaster.WebDav.Providers;

/// <summary>
/// Combines virtual and cached storage providers into a single unified provider.
/// </summary>
public class CombinedStorageProvider : IWebDavStorageProvider
{
    private readonly VirtualStorageProvider _virtualProvider;
    private readonly LocalCacheStorageProvider _cacheProvider;

    public CombinedStorageProvider(VirtualStorageProvider virtualProvider, LocalCacheStorageProvider cacheProvider)
    {
        _virtualProvider = virtualProvider;
        _cacheProvider = cacheProvider;
    }

    public async Task<Stream?> GetFileAsync(string path, CancellationToken cancellationToken)
    {
        return await _virtualProvider.ExistsAsync(path, cancellationToken)
            ? await _virtualProvider.GetFileAsync(path, cancellationToken)
            : await _cacheProvider.GetFileAsync(path, cancellationToken);
    }

    public IAsyncEnumerable<DirectoryEntry> ListDirectoryAsync(string path, CancellationToken cancellationToken)
    {
        return _virtualProvider.ListDirectoryAsync(path, cancellationToken);
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
    {
        return _virtualProvider.ExistsAsync(path, cancellationToken);
    }

    public Task SaveToLocalCacheAsync(string path, Stream content, CancellationToken cancellationToken)
    {
        return _cacheProvider.SaveToLocalCacheAsync(path, content, cancellationToken);
    }

    public async Task CreateDirectoryAsync(string path, CancellationToken cancellationToken)
    {
        if (await _virtualProvider.ExistsAsync(path, cancellationToken))
        {
            throw new NotSupportedException("Cannot create directories in virtual storage.");
        }

        await _cacheProvider.CreateDirectoryAsync(path, cancellationToken);
    }

    public async Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        if (await _virtualProvider.ExistsAsync(path, cancellationToken))
        {
            throw new NotSupportedException("Cannot delete resources in virtual storage.");
        }

        await _cacheProvider.DeleteAsync(path, cancellationToken);
    }

    public async Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
    {
        if (await _virtualProvider.ExistsAsync(sourcePath, cancellationToken))
        {
            throw new NotSupportedException("Cannot copy resources from virtual storage.");
        }

        await _cacheProvider.CopyAsync(sourcePath, destinationPath, cancellationToken);
    }

    public async Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
    {
        if (await _virtualProvider.ExistsAsync(sourcePath, cancellationToken))
        {
            throw new NotSupportedException("Cannot move resources from virtual storage.");
        }

        await _cacheProvider.MoveAsync(sourcePath, destinationPath, cancellationToken);
    }

    public async Task<string?> GetTSStreamUrlAsync(string path, CancellationToken cancellationToken)
    {
        return await _virtualProvider.GetTSStreamUrlAsync(path, cancellationToken);
    }
}