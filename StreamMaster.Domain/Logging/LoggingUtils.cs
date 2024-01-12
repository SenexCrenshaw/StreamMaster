using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Cache;

namespace StreamMaster.Domain.Logging;

public class LoggingUtils(IMemoryCache memoryCache) : ILoggingUtils
{
    private bool? _cleanUrlsCache;

    public string GetLoggableURL(string sourceUrl)
    {
        if (!_cleanUrlsCache.HasValue)
        {
            // This will block the calling thread. Be cautious using this in a main thread or UI thread.
            _cleanUrlsCache = LoadCleanUrlsSettingAsync().Result;
        }

        return _cleanUrlsCache.Value ? "url removed" : sourceUrl;
    }


    private async Task<bool> LoadCleanUrlsSettingAsync()
    {
        Setting settings = memoryCache.GetSetting();
        return settings.CleanURLs;
    }

    public async Task<string> GetLoggableURLAsync(string sourceUrl)
    {
        if (!_cleanUrlsCache.HasValue)
        {
            _cleanUrlsCache = await LoadCleanUrlsSettingAsync();
        }

        return _cleanUrlsCache.Value ? "\'URL Removed\'" : sourceUrl;
    }
}
