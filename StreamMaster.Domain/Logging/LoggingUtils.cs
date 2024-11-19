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
            _cleanUrlsCache = LoadCleanUrlsSetting;
        }

        return _cleanUrlsCache.Value ? "\'URL Removed\'" : sourceUrl;
    }

    private bool LoadCleanUrlsSetting => settings.CleanURLs;
}
