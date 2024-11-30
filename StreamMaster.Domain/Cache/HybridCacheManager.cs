using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Cache;

public class HybridCacheManager<T>(ILogger<HybridCacheManager<T>> logger, IMemoryCache memoryCache, TimeSpan? defaultExpiration = null, bool useCompression = false, bool useKeyBasedFiles = false)
    : IHybridCache<T>, IDisposable
{
    private readonly string cacheDirectory = BuildInfo.SDJSONFolder;
    private readonly string cacheFilePath = Path.Combine(BuildInfo.SDJSONFolder, $"{typeof(T).Name}.json{(useCompression ? ".gz" : "")}");
    private readonly SemaphoreSlim fileLock = new(1, 1);
    private readonly SemaphoreSlim cacheLock = new(1, 1);
    private readonly TimeSpan defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(30);
    private readonly Dictionary<string, CacheEntry<string>> diskCache = [];

    private bool cacheLoaded;
    private bool disposed;

    public async Task<string?> GetAsync(string key)
    {
        if (memoryCache.TryGetValue(key, out string? value))
        {
            return value;
        }

        if (useKeyBasedFiles)
        {
            // Key-based file loading
            await fileLock.WaitAsync();
            try
            {
                string cachePath = GetCacheFilePath(key);
                if (File.Exists(cachePath))
                {
                    await using FileStream fileStream = new(cachePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    if (useCompression)
                    {
                        await using GZipStream decompressionStream = new(fileStream, CompressionMode.Decompress);
                        value = await JsonSerializer.DeserializeAsync<string>(decompressionStream);
                    }
                    else
                    {
                        value = await JsonSerializer.DeserializeAsync<string>(fileStream);
                    }

                    if (!string.IsNullOrEmpty(value))
                    {
                        memoryCache.Set(key, value, defaultExpiration);
                    }

                    return value;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load cache for key {CacheKey}.", key);
            }
            finally
            {
                fileLock.Release();
            }
        }
        else
        {
            // Single file loading
            await LoadDiskCacheIfNeededAsync();

            await cacheLock.WaitAsync();
            try
            {
                if (diskCache.TryGetValue(key, out CacheEntry<string>? entry) && !entry.IsExpired)
                {
                    memoryCache.Set(key, entry.Value, entry.ExpirationTime - DateTime.UtcNow);
                    return entry.Value;
                }
            }
            finally
            {
                cacheLock.Release();
            }
        }

        return null;
    }

    public async Task SetAsync(string key, string value, TimeSpan? slidingExpiration = null)
    {
        DateTime expiration = DateTime.UtcNow + (slidingExpiration ?? defaultExpiration);
        memoryCache.Set(key, value, slidingExpiration ?? defaultExpiration);

        if (useKeyBasedFiles)
        {
            // Key-based file saving
            await fileLock.WaitAsync();
            try
            {
                string cachePath = GetCacheFilePath(key);
                await using FileStream fileStream = new(cachePath, FileMode.Create, FileAccess.Write, FileShare.None);
                if (useCompression)
                {
                    await using GZipStream compressionStream = new(fileStream, CompressionLevel.Optimal);
                    await JsonSerializer.SerializeAsync(compressionStream, value);
                }
                else
                {
                    await JsonSerializer.SerializeAsync(fileStream, value);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save cache for key {CacheKey}.", key);
            }
            finally
            {
                fileLock.Release();
            }
        }
        else
        {
            // Single file saving
            await LoadDiskCacheIfNeededAsync();

            await cacheLock.WaitAsync();
            try
            {
                diskCache[key] = new CacheEntry<string>(value, expiration);
                await SaveDiskCacheAsync();
            }
            finally
            {
                cacheLock.Release();
            }
        }
    }

    public async Task RemoveAsync(string key)
    {
        memoryCache.Remove(key);

        if (useKeyBasedFiles)
        {
            // Remove key-based file
            await fileLock.WaitAsync();
            try
            {
                string cachePath = GetCacheFilePath(key);
                if (File.Exists(cachePath))
                {
                    File.Delete(cachePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to remove cache for key {CacheKey}.", key);
            }
            finally
            {
                fileLock.Release();
            }
        }
        else
        {
            // Remove from single file cache
            await cacheLock.WaitAsync();
            try
            {
                if (diskCache.Remove(key))
                {
                    await SaveDiskCacheAsync();
                }
            }
            finally
            {
                cacheLock.Release();
            }
        }
    }

    private async Task LoadDiskCacheIfNeededAsync()
    {
        if (cacheLoaded || useKeyBasedFiles)
        {
            return;
        }

        await fileLock.WaitAsync();
        try
        {
            if (cacheLoaded)
            {
                return;
            }

            if (!File.Exists(cacheFilePath))
            {
                cacheLoaded = true;
                return;
            }

            try
            {
                await using FileStream fileStream = new(cacheFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (useCompression)
                {
                    await using GZipStream decompressionStream = new(fileStream, CompressionMode.Decompress);
                    diskCache.Clear();

                    Dictionary<string, CacheEntry<string>>? loadedCache =
                        JsonSerializer.Deserialize<Dictionary<string, CacheEntry<string>>>(decompressionStream);

                    if (loadedCache != null)
                    {
                        foreach (KeyValuePair<string, CacheEntry<string>> entry in loadedCache)
                        {
                            diskCache[entry.Key] = entry.Value;
                        }
                    }
                }
                else
                {
                    diskCache.Clear();
                    Dictionary<string, CacheEntry<string>>? loadedCache =
                        JsonSerializer.Deserialize<Dictionary<string, CacheEntry<string>>>(fileStream);

                    if (loadedCache != null)
                    {
                        foreach (KeyValuePair<string, CacheEntry<string>> entry in loadedCache)
                        {
                            diskCache[entry.Key] = entry.Value;
                        }
                    }
                }

                cacheLoaded = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load disk cache.");
            }
        }
        finally
        {
            fileLock.Release();
        }
    }

    private async Task SaveDiskCacheAsync()
    {
        if (useKeyBasedFiles)
        {
            return; // No operation needed if using key-based files.
        }

        await fileLock.WaitAsync();
        try
        {
            await using FileStream fileStream = new(cacheFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            if (useCompression)
            {
                await using GZipStream compressionStream = new(fileStream, CompressionLevel.Optimal);
                await JsonSerializer.SerializeAsync(compressionStream, diskCache);
            }
            else
            {
                await JsonSerializer.SerializeAsync(fileStream, diskCache);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save disk cache.");
        }
        finally
        {
            fileLock.Release();
        }
    }

    private string GetCacheFilePath(string key)
    {
        string cacheKey = GenerateCacheKey(key);
        string name = typeof(T).Name;
        _ = Directory.CreateDirectory(Path.Combine(cacheDirectory, name));
        return Path.Combine(cacheDirectory, name, name + "-" + cacheKey);
    }

    private static string GenerateCacheKey(string command)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        StringBuilder sanitized = new(command.Length);
        foreach (char c in command)
        {
            if (!invalidChars.Contains(c))
            {
                _ = sanitized.Append(c);
            }
            else
            {
                _ = sanitized.Append('_');  // replace invalid chars with underscore or another desired character
            }
        }
        _ = sanitized.Append(".json");
        return sanitized.ToString();
    }

    public async Task<List<string>> GetExpiredKeysAsync()
    {
        List<string> expiredKeys = [];

        if (useKeyBasedFiles)
        {
            // Iterate through individual files to find expired keys
            foreach (string filePath in Directory.GetFiles(cacheDirectory))
            {
                await fileLock.WaitAsync();
                try
                {
                    await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    if (useCompression)
                    {
                        await using GZipStream decompressionStream = new(fileStream, CompressionMode.Decompress);
                        CacheEntry<string>? entry = await JsonSerializer.DeserializeAsync<CacheEntry<string>>(decompressionStream);
                        if (entry?.IsExpired == true)
                        {
                            expiredKeys.Add(Path.GetFileNameWithoutExtension(filePath));
                        }
                    }
                    else
                    {
                        CacheEntry<string>? entry = await JsonSerializer.DeserializeAsync<CacheEntry<string>>(fileStream);
                        if (entry?.IsExpired == true)
                        {
                            expiredKeys.Add(Path.GetFileNameWithoutExtension(filePath));
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to check expiration for file {FilePath}", filePath);
                }
                finally
                {
                    fileLock.Release();
                }
            }
        }
        else
        {
            // Check expired keys in single file cache
            await LoadDiskCacheIfNeededAsync();

            await cacheLock.WaitAsync();
            try
            {
                expiredKeys = diskCache.Where(kv => kv.Value.IsExpired).Select(kv => kv.Key).ToList();
            }
            finally
            {
                cacheLock.Release();
            }
        }

        return expiredKeys;
    }

    public async Task ClearAsync()
    {
        memoryCache.Dispose();

        if (useKeyBasedFiles)
        {
            // Delete all key-based files
            await fileLock.WaitAsync();
            try
            {
                foreach (string filePath in Directory.GetFiles(cacheDirectory))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to clear key-based cache files.");
            }
            finally
            {
                fileLock.Release();
            }
        }
        else
        {
            // Clear single file cache
            await cacheLock.WaitAsync();
            try
            {
                diskCache.Clear();
                await SaveDiskCacheAsync();
            }
            finally
            {
                cacheLock.Release();
            }
        }
    }

    /// <summary>
    /// Checks if a key exists in the cache (memory or disk).
    /// </summary>
    /// <param name="key">The key to check for existence.</param>
    /// <returns>True if the key exists; otherwise, false.</returns>
    public async Task<bool> ExistsAsync(string key)
    {
        // Check memory cache first
        if (memoryCache.TryGetValue(key, out _))
        {
            return true;
        }

        // Check disk cache if using key-based files
        if (useKeyBasedFiles)
        {
            string cachePath = GetCacheFilePath(key);
            if (File.Exists(cachePath))
            {
                return true;
            }
        }
        else
        {
            // Check single file cache
            await LoadDiskCacheIfNeededAsync();
            await cacheLock.WaitAsync();
            try
            {
                if (diskCache.ContainsKey(key))
                {
                    return true;
                }
            }
            finally
            {
                cacheLock.Release();
            }
        }

        return false;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
            fileLock.Dispose();
            cacheLock.Dispose();
        }

        disposed = true;
    }

    private record CacheEntry<TValue>(TValue Value, DateTime ExpirationTime)
    {
        [JsonIgnore]
        public bool IsExpired => DateTime.UtcNow > ExpirationTime;
    }
}