using StreamMasterDomain.Cache;

namespace StreamMasterDomain.Logging;

public static class LoggingUtils
{
    private static readonly Cache<bool> _cleanUrlsCache = new(() => FileUtil.GetSetting().CleanURLs, TimeSpan.FromMinutes(1));
    public static string GetLoggableURL(string sourceUrl)
    {
        bool cleanURLs = _cleanUrlsCache.GetValue();
        return cleanURLs ? "url removed" : sourceUrl;
    }

}
