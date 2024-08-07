using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Logging;

public class LoggingUtils(IOptionsMonitor<Setting> intSettings) : ILoggingUtils
{
    private readonly Setting settings = intSettings.CurrentValue;
    private bool? _cleanUrlsCache;

    public string GetLoggableURL(string sourceUrl)
    {
        if (!_cleanUrlsCache.HasValue)
        {
            // This will block the calling thread. Be cautious using this in a main thread or UI thread.
            _cleanUrlsCache = LoadCleanUrlsSetting;
        }

        return _cleanUrlsCache.Value ? "url removed" : sourceUrl;
    }


    private bool LoadCleanUrlsSetting => settings.CleanURLs;

    public async Task<string> GetLoggableURLAsync(string sourceUrl)
    {
        if (!_cleanUrlsCache.HasValue)
        {
            _cleanUrlsCache = LoadCleanUrlsSetting;
        }

        return _cleanUrlsCache.Value ? "\'URL Removed\'" : sourceUrl;
    }
}
