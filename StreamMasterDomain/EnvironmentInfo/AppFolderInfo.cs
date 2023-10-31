using System.Reflection;

namespace StreamMasterDomain.EnvironmentInfo;

public interface IAppFolderInfo
{
    string AppDataFolder { get; }
    string CacheFolder { get; }
    string IconDataFolder { get; }
    string PlayListEPGFolder { get; }
    string PlayListFolder { get; }
    string PlayListM3UFolder { get; }
    string ProgrammeIconDataFolder { get; }
    string SettingFile { get; }
    string StartUpFolder { get; }
    string TempFolder { get; }
}

public class AppFolderInfo : IAppFolderInfo
{
    private static bool setupDirectories = false;

    public AppFolderInfo()
    {
        CacheFolder = $"{BuildInfo.AppDataFolder}Cache{Path.DirectorySeparatorChar}";

        SDCacheFolder = $"{CacheFolder}SD{Path.DirectorySeparatorChar}";
        PlayListFolder = $"{BuildInfo.AppDataFolder}PlayLists{Path.DirectorySeparatorChar}";
        PlayListEPGFolder = $"{PlayListFolder}EPG{Path.DirectorySeparatorChar}";
        PlayListM3UFolder = $"{PlayListFolder}M3U{Path.DirectorySeparatorChar}";

        IconDataFolder = $"{CacheFolder}Icons{Path.DirectorySeparatorChar}";
        ProgrammeIconDataFolder = $"{CacheFolder}ProgrammeIcons{Path.DirectorySeparatorChar}";

        StartUpFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
        TempFolder = Path.GetTempPath();
        SettingFile = $"{BuildInfo.AppDataFolder}settings.json";
        SetupDirectories();
    }

    public string AppDataFolder { get; private set; }

    public string CacheFolder { get; private set; }

    public string IconDataFolder { get; private set; }

    public string PlayListEPGFolder { get; private set; }
    public string PlayListFolder { get; private set; }
    public string PlayListM3UFolder { get; private set; }
    public string SDCacheFolder { get; private set; }
    public string ProgrammeIconDataFolder { get; private set; }

    public string SettingFile { get; private set; }

    public string StartUpFolder { get; private set; }

    public string TempFolder { get; private set; }

    private static void CreateDir(string directory)
    {
        FileUtil.CreateDirectory(directory);
    }

    private void SetupDirectories()
    {
        if (setupDirectories)
        {
            return;
        }

        setupDirectories = true;
        CreateDir(BuildInfo.AppDataFolder);
        CreateDir(CacheFolder);
        CreateDir(IconDataFolder);
        CreateDir(PlayListFolder);
        CreateDir(PlayListEPGFolder);
        CreateDir(PlayListM3UFolder);
        CreateDir(ProgrammeIconDataFolder);
        CreateDir(SDCacheFolder);
    }
}
