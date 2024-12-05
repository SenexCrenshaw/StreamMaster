using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Cache;

public class SMCacheManager<T>//(ILogger<T> logger, IMemoryCache memoryCache, TimeSpan? defaultExpiration = null, bool useCompression = false, bool useKeyBasedFiles = false, string? defaultKey = null)
    : ISMCache<T>, IDisposable
{
    private readonly string cacheDirectory = BuildInfo.SDJSONFolder;
    private readonly string cacheFilePath;
    private readonly SemaphoreSlim fileLock = new(1, 1);
    private readonly SemaphoreSlim cacheLock = new(1, 1);
    private readonly TimeSpan defaultExpiration = TimeSpan.FromMinutes(30);
    private readonly Dictionary<string, CacheEntry<string>> diskCache = [];
    private readonly string memoryCachePartition = $"{typeof(T).Name}:";
    private readonly ConcurrentQueue<(string Key, string Value)> writeQueue = new();
    private readonly CancellationTokenSource cts = new();
    private readonly Task backgroundFlushTask;
    private readonly TimeSpan flushInterval = TimeSpan.FromSeconds(10);
    private readonly ILogger<T> logger;
    private readonly IMemoryCache memoryCache;
    private readonly bool useCompression;
    private readonly bool useKeyBasedFiles;
    private readonly string? defaultKey;

    private bool cacheLoaded;
    private bool disposed;

    public SMCacheManager(
        ILogger<T> logger,
        IMemoryCache memoryCache,
        TimeSpan? defaultExpiration = null,
        bool useCompression = false,
        bool useKeyBasedFiles = false,
        string? defaultKey = null)
    {
        this.logger = logger;
        this.memoryCache = memoryCache;
        this.defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(30);
        this.useCompression = useCompression;
        this.useKeyBasedFiles = useKeyBasedFiles;
        this.defaultKey = defaultKey;

        cacheFilePath = Path.Combine(cacheDirectory, $"{typeof(T).Name}.json{(useCompression ? ".gz" : "")}");
        backgroundFlushTask = Task.Run(() => BackgroundFlushLoop(cts.Token));
    }
    private string GetMemoryCacheKey(string key)
    {
        return $"{memoryCachePartition}{key}";
    }

    private async Task BackgroundFlushLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(flushInterval, cancellationToken);

                if (!useKeyBasedFiles && !writeQueue.IsEmpty)
                {
                    await ProcessWriteQueueAsync();
                }
            }
            catch (OperationCanceledException)
            {
                // Expected during cancellation
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in background flush loop.");
            }
        }
    }

    public async Task<TValue?> GetAsync<TValue>(string? key = null)
    {
        key ??= defaultKey;
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("A key must be provided, or DefaultKey must be set.");
        }

        // Retrieve raw JSON string
        string? json = await GetStringAsync(key).ConfigureAwait(false);
        if (json is null)
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<TValue>(json);
        }
        catch (JsonException ex)
        {
            //logger.LogError(ex, "Failed to deserialize cache value for key {CacheKey}.", key);
            return default;
        }
    }

    private async Task ProcessWriteQueueAsync()
    {
        await fileLock.WaitAsync();
        try
        {
            // Update the in-memory diskCache with queued items
            while (writeQueue.TryDequeue(out (string Key, string Value) item))
            {
                diskCache[item.Key] = new CacheEntry<string>(item.Value, DateTime.UtcNow);
            }

            // Write the entire diskCache to disk
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
            logger.LogError(ex, "Error processing write queue.");
        }
        finally
        {
            fileLock.Release();
        }
    }

    private async Task<string?> GetStringAsync(string key)
    {
        bool busyDebug = false;
        if (busyDebug)
        {
            logger.LogDebug("Get string for file {file} key {CacheKey}.", cacheFilePath, key);
        }

        string memoryCacheKey = GetMemoryCacheKey(key);
        if (memoryCache.TryGetValue(memoryCacheKey, out string? value))
        {
            //logger.LogDebug("From memory, Get string for key {CacheKey}", memoryCacheKey);
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
                        memoryCache.Set(memoryCacheKey, value, defaultExpiration);
                    }
                    if (busyDebug)
                    {
                        logger.LogDebug("From Keybased, Get string for key {CacheKey}'.", key);
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
                if (diskCache.TryGetValue(key, out CacheEntry<string>? entry))
                {
                    bool a = entry.IsExpired;
                    memoryCache.Set(memoryCacheKey, entry.Value, defaultExpiration);
                    if (busyDebug)
                    {
                        logger.LogDebug("From file, Get string for key {CacheKey}", key);
                    }

                    return entry.Value;
                }
            }
            catch (Exception ex)
            {
                if (busyDebug)
                {
                    logger.LogError(ex, "Failed to load cache for key {CacheKey}.", key);
                }
            }
            finally
            {
                cacheLock.Release();
            }
        }
        logger.LogDebug("Not found {CacheKey} in {file}", key, cacheFilePath);
        return null;
    }

    //public async Task SetAsync<TValue>(string? key, TValue value)
    //{
    //    key ??= defaultKey;
    //    if (string.IsNullOrEmpty(key))
    //    {
    //        throw new ArgumentException("A key must be provided, or DefaultKey must be set.");
    //    }
    //    await SetAsync<TValue>(key, value).ConfigureAwait(false);
    //}

    public async Task SetAsync<TValue>(TValue value, TimeSpan? slidingExpiration = null)
    {
        if (string.IsNullOrEmpty(defaultKey))
        {
            throw new ArgumentException("A key must be provided, or DefaultKey must be set.");
        }
        await SetAsync(defaultKey, value, slidingExpiration).ConfigureAwait(false);
    }
    public async Task SetAsync<TValue>(string? key, TValue value, TimeSpan? slidingExpiration = null)
    {
        key ??= defaultKey;
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("A key must be provided, or DefaultKey must be set.");
        }

        string json;
        try
        {
            json = JsonSerializer.Serialize(value);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to serialize value for cache key {CacheKey}.", key);
            return;
        }

        await SetStringAsync(key, json, slidingExpiration).ConfigureAwait(false);
    }
    private async Task SetStringAsync(string key, string value, TimeSpan? slidingExpiration = null)
    {
        DateTime expiration = DateTime.UtcNow + (slidingExpiration ?? defaultExpiration);
        string memoryCacheKey = GetMemoryCacheKey(key);
        memoryCache.Set(memoryCacheKey, value, expiration);

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
            EnqueueWrite(key, value, slidingExpiration ?? defaultExpiration);
            //await cacheLock.WaitAsync();
            //try
            //{
            //    diskCache[key] = new CacheEntry<string>(value, expiration);
            //    await SaveDiskCacheAsync();
            //}
            //finally
            //{
            //    cacheLock.Release();
            //}
        }
    }

    public async Task SetBulkAsync(Dictionary<string, string> items, TimeSpan? slidingExpiration = null, bool noSave = false)
    {
        foreach (KeyValuePair<string, string> kvp in items)
        {
            string key = kvp.Key;
            string value = kvp.Value;

            string memoryCacheKey = GetMemoryCacheKey(key);
            DateTime expiration = DateTime.UtcNow + (slidingExpiration ?? defaultExpiration);
            memoryCache.Set(memoryCacheKey, value, expiration);

            if (!useKeyBasedFiles)
            {
                await LoadDiskCacheIfNeededAsync();
                await cacheLock.WaitAsync();
                try
                {
                    diskCache[key] = new CacheEntry<string>(value, expiration);
                }
                finally
                {
                    cacheLock.Release();
                }
            }
            else if (!noSave)
            {
                EnqueueWrite(key, value, slidingExpiration);
                //await SaveKeyBasedCacheAsync(key, value, slidingExpiration);
            }
        }

        //if (!useKeyBasedFiles && !noSave)
        //{
        //    await SaveDiskCacheAsync();
        //}
    }

    private void EnqueueWrite(string key, string value, TimeSpan? slidingExpiration = null)
    {
        writeQueue.Enqueue((key, value));
        memoryCache.Set(GetMemoryCacheKey(key), value, slidingExpiration ?? defaultExpiration);
    }

    public async Task SaveAsync()
    {
        //if (useKeyBasedFiles)
        //{
        //    // Key-based files are already saved during `SetAsync`.
        //    return;
        //}

        //await SaveDiskCacheAsync();
    }

    //private async Task SaveKeyBasedCacheAsync(string key, string value, TimeSpan? slidingExpiration = null)
    //{
    //    await fileLock.WaitAsync();
    //    try
    //    {
    //        string cachePath = GetCacheFilePath(key);
    //        await using FileStream fileStream = new(cachePath, FileMode.Create, FileAccess.Write, FileShare.None);
    //        if (useCompression)
    //        {
    //            await using GZipStream compressionStream = new(fileStream, CompressionLevel.Optimal);
    //            await JsonSerializer.SerializeAsync(compressionStream, value);
    //        }
    //        else
    //        {
    //            await JsonSerializer.SerializeAsync(fileStream, value);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "Failed to save key-based cache for key {CacheKey}.", key);
    //    }
    //    finally
    //    {
    //        fileLock.Release();
    //    }
    //}

    public async Task SetAsync(string? key, string value, TimeSpan? slidingExpiration = null)
    {
        key ??= defaultKey;
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("A key must be provided, or DefaultKey must be set.");
        }

        DateTime expiration = DateTime.UtcNow + (slidingExpiration ?? defaultExpiration);
        string memoryCacheKey = GetMemoryCacheKey(key);
        memoryCache.Set(memoryCacheKey, value, slidingExpiration ?? defaultExpiration);

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

            EnqueueWrite(key, value, slidingExpiration);

            //await cacheLock.WaitAsync();
            //try
            //{
            //    diskCache[key] = new CacheEntry<string>(value, expiration);
            //    await SaveDiskCacheAsync();
            //}
            //finally
            //{
            //    cacheLock.Release();
            //}
        }
    }

    public async Task RemoveAsync(string? key)
    {
        key ??= defaultKey;
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("A key must be provided, or DefaultKey must be set.");
        }
        string memoryCacheKey = GetMemoryCacheKey(key);
        memoryCache.Remove(memoryCacheKey);

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
                    //await SaveDiskCacheAsync();
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

    private async Task SaveDiskCacheAsync2()
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
            logger.LogError(ex, "Failed to save disk cache {file}", cacheFilePath);
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
                //await SaveDiskCacheAsync();
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
    public async Task<bool> ExistsAsync(string? key)
    {
        key ??= defaultKey;
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("A key must be provided, or DefaultKey must be set.");
        }
        string memoryCacheKey = GetMemoryCacheKey(key);
        // Check memory cache first
        if (memoryCache.TryGetValue(memoryCacheKey, out _))
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

    private record CacheEntry<TValue>(TValue Value, DateTime LastUpdatedDate, TimeSpan? DefaultExpiration = null)
    {

        [JsonIgnore]
        public bool IsExpired => DateTime.UtcNow > (LastUpdatedDate + (DefaultExpiration ?? TimeSpan.FromMinutes(30)));
    }
}