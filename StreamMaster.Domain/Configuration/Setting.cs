using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Configuration;

public class BaseSettings
{
    public BaseSettings()
    {
        AuthenticationMethod = "None";
    }
    public string AdminPassword { get; set; } = string.Empty;
    public string AdminUserName { get; set; } = string.Empty;
    public string AuthenticationMethod { get; set; }
    public bool AutoSetEPG { get; set; } = true;
    public bool BackupEnabled { get; set; } = true;
    public int BackupInterval { get; set; } = 4;
    public int BackupVersionsToKeep { get; set; } = 18;
    //public bool CacheIcons { get; set; } = true;
    public string LogoCache { get; set; } = "Cache";
    public int IconCacheExpirationDays { get; set; } = 7;
    public bool CleanURLs { get; set; } = true;
    public int ShutDownDelay { get; set; } = 1000;
    public bool ShowMessageVideos { get; set; } = false;
    public string ClientUserAgent { get; set; } = "VLC/3.0.20-git LibVLC/3.0.20-git";

    public string DefaultLogo { get; set; } = "images/default.png";
    public string DefaultCompression { get; set; } = "gz";
    public string DeviceID { get; set; } = "device1";
    public string DummyRegex { get; set; } = "(no tvg-id)";
    public bool EnableSSL { get; set; }

    public bool EnableDBDebug { get; set; } = false;
    public int GlobalStreamLimit { get; set; } = 1;
    public int MaxConcurrentDownloads { get; set; } = 8;
    public int MaxConnectRetry { get; set; } = 20;
    public int MaxConnectRetryTimeMS { get; set; } = 200;
    public int MaxLogFileSizeMB { get; set; } = 1;
    public int MaxLogFiles { get; set; } = 10;
    public int MaxStreamReStart { get; set; } = 3;

    [TsProperty(ForceNullable = true)]
    public List<string> NameRegex { get; set; } = [];
    public bool PrettyEPG { get; set; } = false;
    public bool ShowClientHostNames { get; set; }
    public string ShowIntros { get; set; } = "None";
    public string SSLCertPassword { get; set; } = string.Empty;
    public string SSLCertPath { get; set; } = string.Empty;
    //public string SourceClientUserAgent { get; set; } = "VLC/3.0.20-git LibVLC/3.0.20-git";
    public string UiFolder { get; set; } = "wwwroot";
    public string UrlBase { get; set; } = string.Empty;
    public bool VideoStreamAlwaysUseEPGLogo { get; set; } = true;
}

public class StreamSettings : BaseSettings
{
    public string FFMPegExecutable { get; set; } = "ffmpeg";
    public string FFProbeExecutable { get; set; } = "ffprobe";
    public string DefaultCommandProfileName { get; set; } = "Default";
    public string DefaultOutputProfileName { get; set; } = "Default";
}

public class Setting : StreamSettings
{
    [IgnoreMap]
    public string ServerKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    public int ReadTimeOutMs { get; set; } = 0;
    public int DBBatchSize { get; set; } = 100;
    public int StreamStartTimeoutMs { get; set; } = 0;
}