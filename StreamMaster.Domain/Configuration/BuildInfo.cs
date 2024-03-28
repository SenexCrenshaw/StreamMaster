﻿using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StreamMaster.Domain.Configuration
{
    /// <summary>
    /// Provides information about the build and environment settings.
    /// </summary>
    public static class BuildInfo
    {
        static BuildInfo()
        {
            Assembly? assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Failed to get entry assembly.");
            Version = assembly.GetName().Version ?? new Version(0, 0, 0, 0);
            object[] attributes = assembly.GetCustomAttributes(true);

            Branch = "unknown";
            Release = Version.ToString();

            AssemblyInformationalVersionAttribute? informationalVersion = attributes.OfType<AssemblyInformationalVersionAttribute>().FirstOrDefault();
            if (informationalVersion is not null)
            {
                Release = informationalVersion.InformationalVersion.Contains("Sha")
                    ? informationalVersion.InformationalVersion[..(informationalVersion.InformationalVersion.IndexOf("Sha", StringComparison.Ordinal) - 1)]
                    : informationalVersion.InformationalVersion;
            }
            GetSettingFiles();
        }

        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsOSX => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsFreeBSD => RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);
        public static string StartUpPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;

        private static DateTime _startTime;

        public static DateTime StartTime
        {
            get
            {
                if (_startTime == DateTime.MinValue)
                {
                    _startTime = DateTime.Now;
                }

                return _startTime;
            }
        }

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

        #endregion

        #region Application Information Properties

        public static string AppName { get; } = "StreamMaster";
        public static bool SetIsSystemReady { get; set; } = false;
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
                catch (Exception ex)
                {
                    // Log the exception or handle it as deemed appropriate for your use case.
                    // Log.Warning(ex, "Unable to determine BuildDateTime.");
                    return DateTime.MinValue; // Return a default value indicating failure.
                }
            }
        }

        #endregion

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

        #endregion

        #region File and Directory Path Fields

        public static string AppDataFolder { get; private set; } = IsWindows ? $"c:{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}" : $"{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}";
        public static readonly string DataFolder = Path.Combine(AppDataFolder, "DB");
        public static readonly string CacheFolder = Path.Combine(AppDataFolder, "Cache");
        public static readonly string LogFolder = Path.Combine(AppDataFolder, "Logs");
        public static readonly string PlayListFolder = Path.Combine(AppDataFolder, "PlayLists");
        public static readonly string TVLogoDataFolder = Path.Combine(AppDataFolder, "tv-logos");
        public static readonly string IconDataFolder = Path.Combine(CacheFolder, "Icons");
        public static readonly string ChannelIconDataFolder = Path.Combine(CacheFolder, "ChannelIcons");
        public static readonly string ProgrammeIconDataFolder = Path.Combine(CacheFolder, "ProgrammeIcons");
        public static readonly string SDJSONFolder = Path.Combine(CacheFolder, "SDJson");
        public static readonly string SDStationLogosFolder = Path.Combine(CacheFolder, "SDStationLogos");
        public static readonly string SDStationLogosCacheFolder = Path.Combine(CacheFolder, "SDStationLogosCache");
        public static readonly string SDImagesFolder = Path.Combine(CacheFolder, "SDImages");
        public static readonly string EPGFolder = Path.Combine(PlayListFolder, "EPG");
        public static readonly string M3UFolder = Path.Combine(PlayListFolder, "M3U");
        public static readonly string HLSOutputFolder = Path.Combine(AppDataFolder, "HLS");
        public static readonly string BackupFolder = Path.Combine(AppDataFolder, "Backups");
        public static readonly string SettingsFolder = Path.Combine(AppDataFolder, "Settings");
        public static readonly string RestoreFolder = Path.Combine(AppDataFolder, "Restore");

        public static readonly string SDEPGCacheFile = Path.Combine(SDJSONFolder, "epgCache.json");
        public static readonly string IconDefault = Path.Combine("images", "default.png");
        public static readonly string FFMPEGDefaultOptions = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1";
        public static readonly string LogFilePath = Path.Combine(LogFolder, "StreamMasterAPI.log");

        public static readonly string LoggingFileName = "logsettings.json";
        public static readonly string LoggingSettingsFile = GetSettingFilePath(LoggingFileName, AppDataFolder);

        public static readonly string SettingFileName = "settings.json";
        public static readonly string SettingsFile = GetSettingFilePath(SettingFileName, AppDataFolder);

        public static readonly string SDSettingFileName = "sdsettings.json";
        public static readonly string SDSettingsFile = GetSettingFilePath(SDSettingFileName);

        public static readonly string HLSSettingFileName = "hlssettings.json";
        public static readonly string HLSSettingsFile = GetSettingFilePath(HLSSettingFileName);

        public static readonly string ProfileFileName = "profiles.json";
        public static readonly string ProfileSettingsFile = GetSettingFilePath(ProfileFileName);

        #endregion

        public static List<string> GetSettingFiles()
        {

            Type targetType = typeof(BuildInfo);

            // Get fields marked with [CreateDir] or named "*Folder"
            IEnumerable<string?> fieldPaths = targetType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(f => f.Name.EndsWith("SettingsFile") && f.FieldType == typeof(string))
                            .Select(f => (string)f.GetValue(null));

            // Get properties marked with [CreateDir] or named "*Folder"
            IEnumerable<string?> propertyPaths = targetType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                .Where(p => p.Name.EndsWith("SettingsFile") && p.PropertyType == typeof(string))
                                .Select(p => (string)p.GetValue(null));

            // Combine paths from fields and properties
            List<string> paths = fieldPaths.Concat(propertyPaths).Where(a => !string.IsNullOrEmpty(a)).Order().ToList();

            return paths;
        }

        #region Helper Methods

        private static void Log(string format, params object[] args)
        {
            string message = string.Format(format, args);
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

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

        #endregion
    }
}
