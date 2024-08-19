using StreamMaster.Domain.Models;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace StreamMaster.SchedulesDirect.Cache;

public class EPGCache<T> : IEPGCache<T>
{
    public Dictionary<string, EPGJsonCache> JsonFiles { get; set; } = [];
    private bool isDirty;
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(4);
    private readonly ILogger<EPGCache<T>> logger;
    private readonly ISchedulesDirectDataService schedulesDirectDataService;

    private readonly SDSettings sdsettings = new();

    public EPGCache(ILogger<EPGCache<T>> logger,
                    ISchedulesDirectDataService schedulesDirectDataService,
                    IOptionsMonitor<SDSettings> intSettings)
    {
        sdsettings = intSettings.CurrentValue;
        this.logger = logger;
        this.schedulesDirectDataService = schedulesDirectDataService;
        if (intSettings.CurrentValue.SDEnabled)
        {
            LoadCache();
        }
    }

    private string GetFilename()
    {
        string typeName = typeof(T).Name;
        string ret = Path.Combine(BuildInfo.SDJSONFolder, $"{typeName}.json");
        return ret;
    }
    public dynamic? ReadJsonFile(Type type)
    {
        if (!File.Exists(GetFilename()))
        {
            // logger.LogInformation($"File \"{filepath}\" does not exist.");
            return null;
        }

        try
        {
            byte[] jsonBytes = File.ReadAllBytes(GetFilename());

            JsonSerializerOptions jsonSerializerOptions = new()
            {
                // Add any desired JsonSerializerOptions here
                // For example, to ignore null values during serialization:
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Deserialize(jsonBytes, type, jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to read file \"{GetFilename()}\". Exception: {FileUtil.ReportExceptionMessages(ex)}");
        }
        return null;
    }

    public void LoadCache()
    {
        string fileName = GetFilename();
        logger.LogInformation($"Loading cache file {GetFilename()}");
        dynamic? res = ReadJsonFile(typeof(Dictionary<string, EPGJsonCache>));
        if (res != null)
        {
            JsonFiles = res;
            return;
        }
        JsonFiles = [];
        if (File.Exists(GetFilename()))
        {
            ResetCache();
            logger.LogInformation("The cache file appears to be corrupted and will need to be rebuilt.");
        }
    }

    public bool WriteJsonFile(object obj)
    {
        try
        {
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };

            using FileStream stream = File.Create(GetFilename());
            JsonSerializer.Serialize(stream, obj, jsonSerializerOptions);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to write file \"{GetFilename()}\". Exception: {FileUtil.ReportExceptionMessages(ex)}");
        }
        return false;
    }

    public void SaveCache()
    {
        if (JsonFiles.Count <= 0)
        {
            logger.LogWarning($"Nothing to save for {GetFilename()}");
            return;
        }

        logger.LogInformation($"Saving cache file {GetFilename()}");
        CleanDictionary();
        if (!WriteJsonFile(JsonFiles))
        {
            logger.LogInformation("Deleting cache file to be rebuilt on next update.");
            DeleteFile();
        }
        isDirty = false;
    }

    public bool DeleteFile()
    {
        if (!File.Exists(GetFilename()))
        {
            return true;
        }

        try
        {
            logger.LogInformation($"Deleting cache file {GetFilename()}");
            File.Delete(GetFilename());
            return true;
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Failed to delete file \"{GetFilename()}\". Exception:{FileUtil.ReportExceptionMessages(ex)}");
        }
        return false;
    }

    public void ReleaseCache()
    {
        JsonFiles.Clear();
    }

    public string? GetAsset(string md5)
    {
        JsonFiles[md5].Current = true;
        return JsonFiles[md5].JsonEntry;
    }

    private static string? CleanJsonText(string? json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            json = Regex.Replace(json, "\"\\w+?\":null,?", string.Empty);
            json = Regex.Replace(json, "\"\\w+?\":\"\",?", string.Empty);
            json = Regex.Replace(json, "\"\\w+?\":false,?", string.Empty);
            json = Regex.Replace(json, ",}", "}");
            json = Regex.Replace(json, ",]", "]");
        }
        return json;
    }

    public void AddAsset(string md5, string? json)
    {
        if (JsonFiles.ContainsKey(md5))
        {
            return;
        }

        // reduce the size of the string by removing nulls, empty strings, and false booleans
        json = CleanJsonText(json);

        // store
        EPGJsonCache epgJson = new()
        {
            JsonEntry = json,
            Current = true
        };
        JsonFiles.Add(md5, epgJson);
        isDirty = true;
    }

    public void UpdateAssetImages(string md5, string? json)
    {
        if (!JsonFiles.ContainsKey(md5))
        {
            AddAsset(md5, null);
        }

        // reduce the size of the string by removing nulls and empty strings
        json = CleanJsonText(json);

        // store
        JsonFiles[md5].Images = json;
        isDirty = true;
    }

    public void UpdateAssetJsonEntry(string md5, string? json)
    {
        if (!JsonFiles.ContainsKey(md5))
        {
            AddAsset(md5, json);
        }

        // reduce the size of the string by removing nulls and empty strings
        json = CleanJsonText(json);

        // store
        JsonFiles[md5].JsonEntry = json;
        isDirty = true;
    }

    public void CleanDictionary()
    {
        List<string> keysToDelete = (from asset in JsonFiles where !asset.Value.Current select asset.Key).ToList();
        foreach (string? key in keysToDelete)
        {
            JsonFiles.Remove(key);
        }
        logger.LogInformation($"{keysToDelete.Count} entries deleted from the cache file during cleanup.");
    }

    public void ResetCache()
    {
        if (File.Exists(BuildInfo.SDEPGCacheFile))
        {
            File.Delete(BuildInfo.SDEPGCacheFile);
        }
        JsonFiles = [];
        isDirty = false;
    }

    public async Task<T?> GetValidCachedDataAsync(string name, CancellationToken cancellationToken = default)
    {
        //return default;
        await _cacheSemaphore.WaitAsync(cancellationToken);
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

            return cacheEntry != null && (DateTime.Now - cacheEntry.Timestamp) <= CacheDuration ? cacheEntry.Data : default;
        }
        catch (Exception)
        {
            return default;
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    public async Task WriteToCacheAsync(string name, T data, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken);
        try
        {
            string cacheKey = SDHelpers.GenerateCacheKey(name);
            string cachePath = Path.Combine(BuildInfo.SDJSONFolder, cacheKey);
            SDCacheEntry<T> cacheEntry = new()
            {
                Data = data,
                Command = name,
                Content = "",
                Timestamp = SMDT.UtcNow
            };

            string contentToCache = JsonSerializer.Serialize(cacheEntry);
            await File.WriteAllTextAsync(cachePath, contentToCache, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    public MxfGuideImage? GetGuideImageAndUpdateCache(List<ProgramArtwork>? artwork, ImageType type, string? cacheKey = null)
    {
        if (artwork is null || artwork.Count == 0)
        {
            if (cacheKey != null)
            {
                UpdateAssetImages(cacheKey, string.Empty);
            }

            return null;
        }
        if (cacheKey != null)
        {
            using StringWriter writer = new();
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
            image = artwork.FirstOrDefault(arg => arg.Aspect.ToLower().Equals(aspect));
        }

        if (image == null && type == ImageType.Series)
        {
            image = artwork.FirstOrDefault(arg => arg.Aspect.Equals("4x3", StringComparison.OrdinalIgnoreCase));
        }
        ISchedulesDirectData schedulesDirectData = schedulesDirectDataService.SchedulesDirectData();
        return image != null ? schedulesDirectData.FindOrCreateGuideImage(image.Uri) : null;
    }
}
