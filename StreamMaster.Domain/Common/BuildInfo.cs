using System.Reflection;

namespace StreamMaster.Domain.Common;

public static class BuildInfo
{
    static BuildInfo()
    {
        Assembly? assembly = Assembly.GetEntryAssembly();

        Version = assembly.GetName().Version;

        object[] attributes = assembly.GetCustomAttributes(true);

        Branch = "unknow";
        Release = Version.ToString();

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        LogFilePath = Path.Combine(LogFolder, $"StreamMasterAPI_{timestamp}.log");

        AssemblyInformationalVersionAttribute? informationalVersion = attributes.OfType<AssemblyInformationalVersionAttribute>().FirstOrDefault();
        if (informationalVersion is not null)
        {
            string[] parts = informationalVersion.InformationalVersion.ToString().Split('+');

            if (parts.Length == 2)
            {
                string release = parts[1];
                if (release.Contains("."))
                {
                    release = release[..release.IndexOf(".")];
                }
                Release = $"{parts[0]}-{release}";
            }
        }
    }

    public static string AppName { get; } = "StreamMaster";

    public static bool SetIsSystemReady { get; set; } = false;
    public static Version Version { get; }
    public static string Branch { get; }
    public static string Release { get; }
    //public static string AppDataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.{AppName.ToLower()}{Path.DirectorySeparatorChar}";
    public static string AppDataFolder = "/config/";
    public static readonly string DataFolder = $"{AppDataFolder}";

    public static readonly string CacheFolder = $"{AppDataFolder}Cache{Path.DirectorySeparatorChar}";
    public static readonly string LogFolder = $"{AppDataFolder}Logs{Path.DirectorySeparatorChar}";
    public static readonly string PlayListFolder = $"{AppDataFolder}PlayLists{Path.DirectorySeparatorChar}";

    public static readonly string TVLogoDataFolder = $"{AppDataFolder}tv-logos{Path.DirectorySeparatorChar}";

    public static readonly string IconDataFolder = $"{CacheFolder}Icons{Path.DirectorySeparatorChar}";

    public static readonly string ChannelIconDataFolder = $"{CacheFolder}ChannelIcons{Path.DirectorySeparatorChar}";
    public static readonly string ProgrammeIconDataFolder = $"{CacheFolder}ProgrammeIcons{Path.DirectorySeparatorChar}";

    public static readonly string SDJSONFolder = $"{CacheFolder}SDJson{Path.DirectorySeparatorChar}";
    public static readonly string SDStationLogos = $"{CacheFolder}SDStationLogos{Path.DirectorySeparatorChar}";
    public static readonly string SDStationLogosCache = $"{CacheFolder}SDStationLogosCache{Path.DirectorySeparatorChar}";
    public static readonly string SDImagesFolder = $"{CacheFolder}SDImages{Path.DirectorySeparatorChar}";
    public static readonly string SDEPGCacheFile = $"{SDJSONFolder}epgCache.json";

    public static readonly string EPGFolder = $"{PlayListFolder}EPG{Path.DirectorySeparatorChar}";
    public static readonly string M3UFolder = $"{PlayListFolder}M3U{Path.DirectorySeparatorChar}";

    public static readonly string SettingFile = $"{AppDataFolder}settings.json";
    public static readonly string IconDefault = "images/default.png";
    public static readonly string FFMPEGDefaultOptions = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1";

    public static readonly string LogFilePath = $"{LogFolder}StreamMasterAPI.log";
    public static DateTime BuildDateTime
    {
        get
        {
            string fileLocation = Assembly.GetCallingAssembly().Location;
            return new FileInfo(fileLocation).LastWriteTimeUtc;
        }
    }

    public static bool IsDebug =>
#if DEBUG
            true;
#else
                false;
#endif

}