using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Common;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace StreamMaster.SchedulesDirectAPI;

public class EPGCache(ILogger<EPGCache> logger) : IEPGCache
{
    public Dictionary<string, EPGJsonCache> JsonFiles { get; set; } = [];
    private bool isDirty;

    public dynamic? ReadJsonFile(string filepath, Type type)
    {
        if (!File.Exists(filepath))
        {
            // logger.LogInformation($"File \"{filepath}\" does not exist.");
            return null;
        }

        try
        {
            byte[] jsonBytes = File.ReadAllBytes(filepath);

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                // Add any desired JsonSerializerOptions here
                // For example, to ignore null values during serialization:
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Deserialize(jsonBytes, type, jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to read file \"{filepath}\". Exception: {SDHelpers.ReportExceptionMessages(ex)}");
        }
        return null;
    }



    public void LoadCache()
    {
        var res = ReadJsonFile(BuildInfo.SDEPGCacheFile, typeof(Dictionary<string, EPGJsonCache>));
        if (res != null)
        {
            JsonFiles = res;
            return;
        }
        JsonFiles = [];
        if (File.Exists(BuildInfo.SDEPGCacheFile))
        {
            logger.LogInformation("The cache file appears to be corrupted and will need to be rebuilt.");
        }
    }

    public bool WriteJsonFile(object obj, string filepath)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                // Add any desired JsonSerializerOptions here
                // For example, to ignore null values during serialization:
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false // Formatting.None equivalent
            };

            using var stream = File.Create(filepath);
            JsonSerializer.Serialize(stream, obj, jsonSerializerOptions);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to write file \"{filepath}\". Exception: {SDHelpers.ReportExceptionMessages(ex)}");
        }
        return false;
    }

    public void WriteCache()
    {
        if (!isDirty || JsonFiles.Count <= 0) return;
        CleanDictionary();
        if (!WriteJsonFile(JsonFiles, BuildInfo.SDEPGCacheFile))
        {
            logger.LogInformation("Deleting cache file to be rebuilt on next update.");
            DeleteFile(BuildInfo.SDEPGCacheFile);
        }
        CloseCache();
    }

    public bool DeleteFile(string filepath)
    {
        if (!File.Exists(filepath)) return true;
        try
        {
            File.Delete(filepath);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Failed to delete file \"{filepath}\". Exception:{SDHelpers.ReportExceptionMessages(ex)}");
        }
        return false;
    }

    public void CloseCache()
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
        if (JsonFiles.ContainsKey(md5)) return;

        // reduce the size of the string by removing nulls, empty strings, and false booleans
        json = CleanJsonText(json);

        // store
        var epgJson = new EPGJsonCache()
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
        var keysToDelete = (from asset in JsonFiles where !asset.Value.Current select asset.Key).ToList();
        foreach (var key in keysToDelete)
        {
            JsonFiles.Remove(key);
        }
        logger.LogInformation($"{keysToDelete.Count} entries deleted from the cache file during cleanup.");
    }
}

