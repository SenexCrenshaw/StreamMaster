namespace StreamMaster.WebDav.Services;

/// <summary>
/// Manages local file caching, ensuring persistence and quick access.
/// </summary>
public class LocalFileCacheManager : ILocalFileCacheManager
{
    private readonly string _cacheDirectory;

    public LocalFileCacheManager(string cacheDirectory)
    {
        _cacheDirectory = cacheDirectory;
        Directory.CreateDirectory(_cacheDirectory);
    }

    public async Task SaveToCacheAsync(string path, Stream content, CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_cacheDirectory, path);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using FileStream fileStream = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fileStream, cancellationToken);
    }

    public Task<Stream?> GetFromCacheAsync(string path, CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_cacheDirectory, path);
        return File.Exists(fullPath)
            ? Task.FromResult<Stream?>(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            : Task.FromResult<Stream?>(null);
    }

    public Task<bool> ExistsInCacheAsync(string path, CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_cacheDirectory, path);
        return Task.FromResult(File.Exists(fullPath));
    }
}