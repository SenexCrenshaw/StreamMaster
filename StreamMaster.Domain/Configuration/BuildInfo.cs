using StreamMaster.Domain.Extensions;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Configuration
{
    /// <summary>
    /// Provides information about the build and environment settings.
    /// </summary>
    public static class BuildInfo
    {
        public static JsonSerializerOptions JsonIndentOptions = new() { WriteIndented = true };
        public static JsonSerializerOptions JsonIndentOptionsWhenWritingNull = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, WriteIndented = false };

        static BuildInfo()
        {
            Assembly? assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Failed to get entry assembly.");
            Version = assembly.GetName().Version ?? new Version(0, 0, 0, 0);
            object[] attributes = assembly.GetCustomAttributes(true);
            StartTime = SMDT.UtcNow;
            Branch = "unknown";
            Release = Version.ToString();

            AssemblyInformationalVersionAttribute? informationalVersion = attributes.OfType<AssemblyInformationalVersionAttribute>().FirstOrDefault();
            if (informationalVersion is not null)
            {
                Release = informationalVersion.InformationalVersion.Contains("Sha")
                    ? informationalVersion.InformationalVersion[..(informationalVersion.InformationalVersion.IndexOf("Sha", StringComparison.Ordinal) - 1)]
                    : informationalVersion.InformationalVersion;
            }
            _ = GetSettingFiles();
        }

        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsFreeBSD => RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);
        public static string StartUpPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;

        //public const int DBBatchSize = 500;
        public static DateTime StartTime { get; set; }

        #region Database Configuration Properties

        /// <summary>
        /// Database name, fetched from environment variable or default if not set.
        /// </summary>
        public static string DBName => GetEnvironmentVariableOrDefault("POSTGRES_DB", "StreamMaster");

        public static string DBHost => GetEnvironmentVariableOrDefault("POSTGRES_HOST", "127.0.0.1");

        /// <summary>
        /// Database user, fetched from environment variable or default if not set.
        /// </summary>
        public static string DBUser => GetEnvironmentVariableOrDefault("POSTGRES_USER", "postgres");

        /// <summary>
        /// Database password, fetched from environment variable or default if not set.
        /// </summary>
        public static string DBPassword => GetEnvironmentVariableOrDefault("POSTGRES_PASSWORD", "sm123");

        #endregion Database Configuration Properties

        #region Application Information Properties

        public static string AppName { get; } = "StreamMaster";
        public static bool IsSystemReady { get; set; } = false;
        public static bool IsTaskRunning { get; set; } = false;
        public static Version Version { get; }
        public static string Branch { get; }
        public static string Release { get; }

        /// <summary>
        /// Gets the build date and time by reading the last write time of the assembly.
        /// </summary>
        public static DateTime BuildDateTime
        {
            get
            {
                try
                {
                    string? assemblyLocation = Assembly.GetExecutingAssembly().Location;
                    return string.IsNullOrEmpty(assemblyLocation) || !File.Exists(assemblyLocation)
                        ? throw new FileNotFoundException("Failed to locate the executing assembly.")
                        : new FileInfo(assemblyLocation).LastWriteTimeUtc;
                }
                catch (Exception)
                {
                    // Log the exception or handle it as deemed appropriate for your use case.
                    // Log.Warning(ex, "Unable to determine BuildDateTime.");
                    return DateTime.MinValue; // Return a default value indicating failure.
                }
            }
        }

        #endregion Application Information Properties

        #region Build Configuration Property

        /// <summary>
        /// Indicates if the application is running in Debug mode.
        /// </summary>
        public static bool IsDebug =>
#if DEBUG
            true;

#else
            false;
#endif

        #endregion Build Configuration Property

        #region File and Directory Path Fields

        public static string AppDataFolder { get; } = IsWindows ? $"c:{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}" : $"{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}";
        public static readonly int BufferSize = 4096;

        public static readonly string DataFolder = Path.Combine(AppDataFolder, "DB");
        public static readonly string CacheFolder = Path.Combine(AppDataFolder, "Cache");
        public static readonly string LogFolder = Path.Combine(AppDataFolder, "Logs");
        public static readonly string PlayListFolder = Path.Combine(AppDataFolder, "PlayLists");
        public static readonly string TVLogoFolder = Path.Combine(AppDataFolder, "tv-logos");
        public static readonly string LogoFolder = Path.Combine(CacheFolder, "Logos");
        public static readonly string DupDataFolder = Path.Combine(CacheFolder, "DuplicateStreamLists");

        //public static readonly string ProgrammeIconDataFolder = Path.Combine(CacheFolder, "ProgrammeIcons");
        public static readonly string SDJSONFolder = Path.Combine(CacheFolder, "SDJson");

        public static readonly string SDStationLogosFolder = Path.Combine(CacheFolder, "SDStationLogos");
        public static readonly string SDStationLogosCacheFolder = Path.Combine(CacheFolder, "SDStationLogosCache");

        public static readonly string SDImagesFolder = Path.Combine(CacheFolder, "SDImages");
        public static readonly string EPGFolder = Path.Combine(PlayListFolder, "EPG");
        public static readonly string M3UFolder = Path.Combine(PlayListFolder, "M3U");

        //public static readonly string HLSOutputFolder = Path.Combine(AppDataFolder, "HLS");
        public static readonly string BackupFolder = Path.Combine(AppDataFolder, "Backups");

        public static readonly string SettingsFolder = Path.Combine(AppDataFolder, "Settings");
        public static readonly string RestoreFolder = Path.Combine(AppDataFolder, "Restore");

        public static readonly string CustomPlayListFolder = Path.Combine(AppDataFolder, "CustomPlayList");

        public static readonly string IntrosFolder = Path.Combine(AppDataFolder, "Intros");
        public static readonly string MessagesFolder = Path.Combine(AppDataFolder, "Messages");
        public static readonly string MessageNoStreamsLeft = Path.Combine(MessagesFolder, "NoStreamsLeft.mp4");

        public static readonly string SDEPGCacheFile = Path.Combine(SDJSONFolder, "epgCache.json");
        public static readonly string LogoDefault = Path.Combine("images", "default.png");

        public static readonly string LogFileName = "StreamMasterAPI";
        public static readonly string LogFilePath = Path.Combine(LogFolder, LogFileName + ".log");
        public static readonly string LogFileLoggerPath = Path.Combine(LogFolder, "StreamMasterAPIFile.log");

        public static readonly string LoggingFileName = "logsettings.json";
        public static readonly string LoggingSettingsFile = GetSettingFilePath(LoggingFileName, AppDataFolder);

        public static readonly string SettingFileName = "settings.json";
        public static readonly string SettingsFile = GetSettingFilePath(SettingFileName, AppDataFolder);

        public static readonly string SDSettingFileName = "sdsettings.json";
        public static readonly string SDSettingsFile = GetSettingFilePath(SDSettingFileName);

        //public static readonly string HLSSettingFileName = "hlssettings.json";
        //public static readonly string HLSSettingsFile = GetSettingFilePath(HLSSettingFileName);
        //public static readonly string HLSLogFolder = Path.Combine(LogFolder, "HLS");

        public static readonly string CommandProfileFileName = "commandprofiles.json";
        public static readonly string CommandProfileSettingsFile = GetSettingFilePath(CommandProfileFileName);

        public static readonly string OutputProfileFileName = "outputprofiles.json";
        public static readonly string OutputProfileSettingsFile = GetSettingFilePath(OutputProfileFileName);

        #endregion File and Directory Path Fields

        public static List<string> GetSettingFiles()
        {
            Type targetType = typeof(BuildInfo);

            // Get fields marked with [CreateDir] or named "*Folder"
            IEnumerable<string?> fieldPaths = targetType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(f => f.Name.EndsWith("SettingsFile") && f.FieldType == typeof(string))
                            .Select(f => (string?)f.GetValue(null));

            // Get properties marked with [CreateDir] or named "*Folder"
            IEnumerable<string?> propertyPaths = targetType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where(p => p.Name.EndsWith("SettingsFile") && p.PropertyType == typeof(string))
                                .Select(p => (string?)p.GetValue(null));

            // Combine paths from fields and properties
            List<string> paths = [.. fieldPaths.Concat(propertyPaths).Where(a => !string.IsNullOrEmpty(a)).Order()];

            return paths;
        }

        #region Helper Methods

        //private static void Log(string format, params object[] args)
        //{
        //    string message = string.Format(format, args);
        //    Console.WriteLine(message);
        //    Debug.WriteLine(message);
        //}

        private static string GetSettingFilePath(string settingFileName, string? folder = null)
        {
            if (string.IsNullOrEmpty(folder))
            {
                folder = SettingsFolder;
            }
            return Path.Combine(folder, settingFileName);

            //return File.Exists(Path.Combine(folder, settingFileName)) ? Path.Combine(folder, settingFileName) : settingFileName;
        }

        ///// <summary>
        ///// Initializes paths for various application data folders.
        ///// </summary>
        //private static void InitializePaths()
        //{
        //    AppDataFolder = $"{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}";
        //    // Initialize other paths based on AppDataFolder...
        //}

        /// <summary>
        /// Gets an environment variable's value or a default value if not set.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="defaultValue">The default value to return if the environment variable is not set.</param>
        /// <returns>The environment variable's value or the default value.</returns>
        private static string GetEnvironmentVariableOrDefault(string name, string defaultValue)
        {
            string? envVar = Environment.GetEnvironmentVariable(name);
            return !string.IsNullOrEmpty(envVar) ? envVar : defaultValue;
        }

        #endregion Helper Methods
    }
}