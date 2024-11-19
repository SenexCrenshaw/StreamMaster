using System.Text.Json;
using System.Text.RegularExpressions;

using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect.Cache;

public partial class EPGCache<T> : IEPGCache<T>
{
    public Dictionary<string, EPGJsonCache> JsonFiles { get; set; } = [];
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(4);
    private readonly ILogger<EPGCache<T>> logger;
    private readonly ISchedulesDirectDataService schedulesDirectDataService;
    private readonly SDSettings sdsettings;

    [GeneratedRegex("\"\\w+?\":null,?")]
    private static partial Regex NullPropertyRegex();

    [GeneratedRegex("\"\\w+?\":\"\",?")]
    private static partial Regex EmptyStringPropertyRegex();

    [GeneratedRegex("\"\\w+?\":false,?")]
    private static partial Regex FalsePropertyRegex();

    [GeneratedRegex(",}")]
    private static partial Regex TrailingCommaObjectRegex();

    [GeneratedRegex(",]")]
    private static partial Regex TrailingCommaArrayRegex();

    public EPGCache(ILogger<EPGCache<T>> logger,
                    ISchedulesDirectDataService schedulesDirectDataService,
                    IOptionsMonitor<SDSettings> intSettings)
    {
        sdsettings = intSettings.CurrentValue;
        this.logger = logger;
        this.schedulesDirectDataService = schedulesDirectDataService;
        if (sdsettings.SDEnabled)
        {
            LoadCache();
        }
    }

    private static string GetFilename()
    {
        string typeName = typeof(T).Name;
        return Path.Combine(BuildInfo.SDJSONFolder, $"{typeName}.json");
    }

    public dynamic? ReadJsonFile(Type type)
    {
        string filename = GetFilename();
        if (!File.Exists(filename))
        {
            return null;
        }

        try
        {
            byte[] jsonBytes = File.ReadAllBytes(filename);

            return JsonSerializer.Deserialize(jsonBytes, type, BuildInfo.JsonIndentOptionsWhenWritingNull);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read file \"{Filename}\"", filename);
        }
        return null;
    }

    public void LoadCache()
    {
        string filename = GetFilename();
        logger.LogInformation("Loading cache file {Filename}", filename);
        dynamic? res = ReadJsonFile(typeof(Dictionary<string, EPGJsonCache>));
        if (res != null)
        {
            JsonFiles = res;
            return;
        }
        JsonFiles = [];
        if (File.Exists(filename))
        {
            ResetCache();
            logger.LogWarning("The cache file appears to be corrupted and will need to be rebuilt.");
        }
    }

    public bool WriteJsonFile(object obj)
    {
        try
        {
            using FileStream stream = File.Create(GetFilename());
            JsonSerializer.Serialize(stream, obj, BuildInfo.JsonIndentOptionsWhenWritingNull);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to write file \"{Filename}\"", GetFilename());
        }
        return false;
    }

    public void SaveCache()
    {
        if (JsonFiles.Count == 0)
        {
            logger.LogWarning("Nothing to save for {Filename}", GetFilename());
            return;
        }

        logger.LogInformation("Saving cache file {Filename}", GetFilename());
        CleanDictionary();
        if (!WriteJsonFile(JsonFiles))
        {
            logger.LogWarning("Deleting cache file to be rebuilt on next update.");
            DeleteFile();
        }
    }

    public bool DeleteFile()
    {
        string filename = GetFilename();
        if (!File.Exists(filename))
        {
            return true;
        }

        try
        {
            logger.LogInformation("Deleting cache file {Filename}", filename);
            File.Delete(filename);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete file \"{Filename}\"", filename);
        }
        return false;
    }

    public void ReleaseCache()
    {
        JsonFiles.Clear();
    }

    public string? GetAsset(string md5)
    {
        if (JsonFiles.TryGetValue(md5, out EPGJsonCache? cache))
        {
            cache.Current = true;
            return cache.JsonEntry;
        }
        return null;
    }

    private static string? CleanJsonText(string? json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            json = NullPropertyRegex().Replace(json, string.Empty);
            json = EmptyStringPropertyRegex().Replace(json, string.Empty);
            json = FalsePropertyRegex().Replace(json, string.Empty);
            json = TrailingCommaObjectRegex().Replace(json, "}");
            json = TrailingCommaArrayRegex().Replace(json, "]");
        }
        return json;
    }

    public void AddAsset(string md5, string? json)
    {
        if (JsonFiles.ContainsKey(md5))
        {
            return;
        }

        json = CleanJsonText(json);

        EPGJsonCache epgJson = new()
        {
            JsonEntry = json,
            Current = true
        };
        JsonFiles.Add(md5, epgJson);
    }

    public void UpdateAssetImages(string md5, string? json)
    {
        if (!JsonFiles.ContainsKey(md5))
        {
            AddAsset(md5, null);
        }

        json = CleanJsonText(json);
        JsonFiles[md5].Images = json;
    }

    public void UpdateAssetJsonEntry(string md5, string? json)
    {
        if (!JsonFiles.ContainsKey(md5))
        {
            AddAsset(md5, json);
        }

        json = CleanJsonText(json);
        JsonFiles[md5].JsonEntry = json;
    }

    public void CleanDictionary()
    {
        List<string> keysToDelete = JsonFiles.Where(asset => !asset.Value.Current).Select(asset => asset.Key).ToList();
        foreach (string key in keysToDelete)
        {
            JsonFiles.Remove(key);
        }
        logger.LogInformation("{Count} entries deleted from the cache file during cleanup.", keysToDelete.Count);
    }

    public void ResetCache()
    {
        string filename = BuildInfo.SDEPGCacheFile;
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }
        JsonFiles.Clear();
    }

    public async Task<T?> GetValidCachedDataAsync(string name, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            string cacheKey = SDHelpers.GenerateCacheKey(name);
            string cachePath = Path.Combine(BuildInfo.SDJSONFolder, cacheKey);
            if (!File.Exists(cachePath))
            {
                return default;
            }

            string cachedContent = await File.ReadAllTextAsync(cachePath, cancellationToken).ConfigureAwait(false);
            SDCacheEntry<T>? cacheEntry = JsonSerializer.Deserialize<SDCacheEntry<T>>(cachedContent);

            return cacheEntry != null && (DateTime.UtcNow - cacheEntry.Timestamp) <= CacheDuration ? cacheEntry.Data : default;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while retrieving cached data for {Name}", name);
            return default;
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    public async Task WriteToCacheAsync(string name, T data, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            string cacheKey = SDHelpers.GenerateCacheKey(name);
            string cachePath = Path.Combine(BuildInfo.SDJSONFolder, cacheKey);
            SDCacheEntry<T> cacheEntry = new()
            {
                Data = data,
                Command = name,
                Content = string.Empty,
                Timestamp = DateTime.UtcNow
            };

            string contentToCache = JsonSerializer.Serialize(cacheEntry);
            await File.WriteAllTextAsync(cachePath, contentToCache, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while writing data to cache for {Name}", name);
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    public MxfGuideImage? GetGuideImageAndUpdateCache(List<ProgramArtwork>? artwork, ImageType type, string? cacheKey = null)
    {
        if (artwork == null || artwork.Count == 0)
        {
            if (cacheKey != null)
            {
                UpdateAssetImages(cacheKey, string.Empty);
            }
            return null;
        }

        if (cacheKey != null)
        {
            string artworkJson = JsonSerializer.Serialize(artwork);
            UpdateAssetImages(cacheKey, artworkJson);
        }

        ProgramArtwork? image = null;
        if (type == ImageType.Movie)
        {
            image = artwork.FirstOrDefault();
        }
        else
        {
            string aspect = sdsettings.SeriesPosterArt ? "2x3" : sdsettings.SeriesWsArt ? "16x9" : sdsettings.SeriesPosterAspect;
            image = artwork.FirstOrDefault(arg => arg.Aspect.EndsWithIgnoreCase(aspect));
        }

        if (image == null && type == ImageType.Series)
        {
            image = artwork.FirstOrDefault(arg => arg.Aspect.EndsWithIgnoreCase("4x3"));
        }

        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        return image != null ? schedulesDirectData.FindOrCreateGuideImage(image.Uri) : null;
    }
}
