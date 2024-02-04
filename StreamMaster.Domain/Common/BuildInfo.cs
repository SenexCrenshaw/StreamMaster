using System.Reflection;

namespace StreamMaster.Domain.Common;

public static class BuildInfo
{
    static BuildInfo()
    {
        Assembly? assembly = Assembly.GetEntryAssembly();

        Version = assembly.GetName().Version;
        //    string informationalVersion = assembly
        //.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
        //.OfType<AssemblyInformationalVersionAttribute>()
        //.FirstOrDefault()
        //?.InformationalVersion;

        //    string productVersion = assembly
        //        .GetCustomAttributes(typeof(AssemblyProductAttribute), false)
        //        .OfType<AssemblyProductAttribute>()
        //        .FirstOrDefault()
        //        ?.Product;

        object[] attributes = assembly.GetCustomAttributes(true);

        Branch = "unknow";
        Release = Version.ToString();

        //string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        //LogFilePath = Path.Combine(LogFolder, $"StreamMasterAPI_{timestamp}.log");

        AssemblyInformationalVersionAttribute? informationalVersion = attributes.OfType<AssemblyInformationalVersionAttribute>().FirstOrDefault();
        if (informationalVersion is not null)
        {
            Release = informationalVersion.InformationalVersion.Contains("Sha")
            ? informationalVersion.InformationalVersion[..(informationalVersion.InformationalVersion.IndexOf("Sha") - 1)]
            : informationalVersion.InformationalVersion;


        }
    }

    public static string DBName
    {
        get
        {
            string envHost = Environment.GetEnvironmentVariable("POSTGRES_DB");
            return !string.IsNullOrEmpty(envHost) ? envHost : "StreamMaster";
        }

    }

    public static string DBUser
    {
        get
        {
            string envHost = Environment.GetEnvironmentVariable("POSTGRES_USER");
            return !string.IsNullOrEmpty(envHost) ? envHost : "postgres";
        }

    }

    public static string DBPassword
    {
        get
        {
            string envHost = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            return !string.IsNullOrEmpty(envHost) ? envHost : "sm123";
        }

    }

    public static string AppName { get; } = "StreamMaster";

    public static bool SetIsSystemReady { get; set; } = false;
    public static Version Version { get; }
    public static string Branch { get; }
    public static string Release { get; }
    //public static string AppDataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.{AppName.ToLower()}{Path.DirectorySeparatorChar}";
    public static string AppDataFolder = "/config/";
    public static readonly string DataFolder = Path.Combine(AppDataFolder, "DB");

    public static readonly string CacheFolder = $"{AppDataFolder}Cache{Path.DirectorySeparatorChar}";
    public static readonly string LogFolder = Path.Combine(AppDataFolder, "Logs");
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

    public static readonly string SettingFileName = "settings.json";

    public static readonly string LoggingFileName = "logsettings.json";
    public static readonly string SettingFile = $"{AppDataFolder}{SettingFileName}";


    public static readonly string LoggingFile = File.Exists(Path.Combine(AppDataFolder, LoggingFileName)) ? Path.Combine(AppDataFolder, LoggingFileName) : LoggingFileName;

    public static readonly string IconDefault = "images/default.png";
    public static readonly string FFMPEGDefaultOptions = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1";

    public static readonly string LogFilePath = Path.Combine(LogFolder, "StreamMasterAPI.log");
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