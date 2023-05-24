namespace StreamMasterDomain.Common;

public static class Constants
{
    public static readonly string AppName = "StreamMaster";
    public static readonly string ConfigFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.{AppName.ToLower()}{Path.DirectorySeparatorChar}";

    public static readonly string CacheDirectory = $"{ConfigFolder}Cache{Path.DirectorySeparatorChar}";
    public static readonly string DataDirectory = $"{ConfigFolder}";

    public static readonly string IconDataDirectory = $"{CacheDirectory}Icons{Path.DirectorySeparatorChar}";
    public static readonly string ProgrammeIconDataDirectory = $"{CacheDirectory}ProgrammeIcons{Path.DirectorySeparatorChar}";

    public static readonly string PlayListDirectory = $"{ConfigFolder}PlayLists{Path.DirectorySeparatorChar}";
    public static readonly string TVLogoDirectory = $"{ConfigFolder}tv-logos{Path.DirectorySeparatorChar}";

    public static readonly string EPGDirectory = $"{PlayListDirectory}EPG{Path.DirectorySeparatorChar}";
    public static readonly string M3UDirectory = $"{PlayListDirectory}M3U{Path.DirectorySeparatorChar}";

    public static readonly string SettingFile = $"{ConfigFolder}settings.json";

    public static readonly string IconDefault = "images/default.png";
    public static readonly string EPGExtension = ".xml";
    public static readonly string M3UExtension = ".m3u";
    public static readonly string TvLogoConfig = $"{CacheDirectory}tvlogos.json";

}