using System.Runtime.CompilerServices;

using StreamMaster.WebDav.Domain.Models;

namespace StreamMaster.WebDav.Providers;

public class LocalCacheStorageProvider : IWebDavStorageProvider
{
    private readonly string _cacheRoot;

    public LocalCacheStorageProvider(string cacheRoot)
    {
        _cacheRoot = cacheRoot;
        Directory.CreateDirectory(_cacheRoot);
    }

    public async Task<Stream?> GetFileAsync(string path, CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_cacheRoot, path.TrimStart('/'));
        return !File.Exists(fullPath) ? null : (Stream)new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public async IAsyncEnumerable<DirectoryEntry> ListDirectoryAsync(string path, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_cacheRoot, path.TrimStart('/'));
        if (!Directory.Exists(fullPath))
        {
            yield break;
        }

        foreach (string entry in Directory.EnumerateFileSystemEntries(fullPath))
        {
            bool isDirectory = Directory.Exists(entry);
            FileSystemInfo info = isDirectory ? new DirectoryInfo(entry) : new FileInfo(entry);

            yield return new DirectoryEntry
            {
                Name = info.Name,
                Path = entry.Replace(_cacheRoot, "").Replace("\\", "/"),
                IsDirectory = isDirectory,
                Size = isDirectory ? null : ((FileInfo)info).Length,
            };
        }
    }

    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_cacheRoot, path.TrimStart('/'));
        return Task.FromResult(File.Exists(fullPath) || Directory.Exists(fullPath));
    }

    public async Task SaveToLocalCacheAsync(string path, Stream content, CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_cacheRoot, path.TrimStart('/'));
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using FileStream fileStream = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fileStream, cancellationToken);
    }

    public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_cacheRoot, path.TrimStart('/'));
        Directory.CreateDirectory(fullPath);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        string fullPath = Path.Combine(_cacheRoot, path.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        else if (Directory.Exists(fullPath))
        {
            Directory.Delete(fullPath, true);
        }
        return Task.CompletedTask;
    }

    public Task CopyAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
    {
        string sourceFullPath = Path.Combine(_cacheRoot, sourcePath.TrimStart('/'));
        string destinationFullPath = Path.Combine(_cacheRoot, destinationPath.TrimStart('/'));

        if (File.Exists(sourceFullPath))
        {
            File.Copy(sourceFullPath, destinationFullPath, overwrite: true);
        }
        else if (Directory.Exists(sourceFullPath))
        {
            DirectoryCopy(sourceFullPath, destinationFullPath);
        }
        return Task.CompletedTask;
    }

    public Task MoveAsync(string sourcePath, string destinationPath, CancellationToken cancellationToken)
    {
        string sourceFullPath = Path.Combine(_cacheRoot, sourcePath.TrimStart('/'));
        string destinationFullPath = Path.Combine(_cacheRoot, destinationPath.TrimStart('/'));

        if (File.Exists(sourceFullPath) || Directory.Exists(sourceFullPath))
        {
            Directory.Move(sourceFullPath, destinationFullPath);
        }
        return Task.CompletedTask;
    }

    private static void DirectoryCopy(string sourceDir, string destDir)
    {
        DirectoryInfo dir = new(sourceDir);
        DirectoryInfo[] dirs = dir.GetDirectories();

        Directory.CreateDirectory(destDir);

        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destDir, file.Name);
            file.CopyTo(targetFilePath, overwrite: true);
        }

        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestDir = Path.Combine(destDir, subDir.Name);
            DirectoryCopy(subDir.FullName, newDestDir);
        }
    }

    public Task<string?> GetTSStreamUrlAsync(string path, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}