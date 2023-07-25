using System.Reflection;

namespace StreamMasterDomain.Common;

public static class BuildInfo
{
    static BuildInfo()
    {
        var assembly = Assembly.GetEntryAssembly();

        Version = assembly.GetName().Version;

        var attributes = assembly.GetCustomAttributes(true);

        Branch = "unknow";
        Release = Version.ToString();

        var informationalVersion = attributes.OfType<AssemblyInformationalVersionAttribute>().FirstOrDefault();
        if (informationalVersion is not null)
        {
            string[] parts = informationalVersion.InformationalVersion.ToString().Split('+');

            if (parts.Length == 2)
            {
                var release = parts[1];
                if (release.Contains("."))
                {
                    release = release.Substring(0, release.IndexOf("."));
                }
                Release = $"{parts[0]}-{release}";
            }
        }
    }

    public static string AppName { get; } = "Stream Master";

    public static Version Version { get; }
    public static string Branch { get; }
    public static string Release { get; }
    public static string AppDataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}{Path.DirectorySeparatorChar}.{Constants.AppName.ToLower()}{Path.DirectorySeparatorChar}";
    public static DateTime BuildDateTime
    {
        get
        {
            var fileLocation = Assembly.GetCallingAssembly().Location;
            return new FileInfo(fileLocation).LastWriteTimeUtc;
        }
    }

    public static bool IsDebug
    {
        get
        {
#if DEBUG
            return true;
#else
                return false;
#endif
        }
    }
}