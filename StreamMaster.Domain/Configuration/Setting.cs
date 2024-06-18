using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Configuration;

public class BaseSettings
{

    public bool BackupEnabled { get; set; } = true;
    public int BackupVersionsToKeep { get; set; } = 18;
    public int BackupInterval { get; set; } = 4;
    public bool PrettyEPG { get; set; } = false;
    public int MaxLogFiles { get; set; } = 10;
    public int MaxLogFileSizeMB { get; set; } = 1;
    public bool EnablePrometheus { get; set; } = false;
    public int MaxStreamReStart { get; set; } = 3;
    public int MaxConcurrentDownloads { get; set; } = 8;

    public string AdminPassword { get; set; } = string.Empty;
    public string AdminUserName { get; set; } = string.Empty;
    public string DefaultIcon { get; set; } = "images/default.png";
    public string UiFolder { get; set; } = "wwwroot";
    public string UrlBase { get; set; } = string.Empty;
    public List<string> LogPerformance { get; set; } = ["*.Queries"];
    public string ApiKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    public AuthenticationType AuthenticationMethod { get; set; } = AuthenticationType.None;
    public bool CacheIcons { get; set; } = true;
    public bool CleanURLs { get; set; } = true;
    public string ClientUserAgent { get; set; } = "VLC/3.0.20-git LibVLC/3.0.20-git";
    public string DeviceID { get; set; } = "device1";
    public string DummyRegex { get; set; } = "(no tvg-id)";

    public bool EnableSSL { get; set; }
    public string FFMPegExecutable { get; set; } = "ffmpeg";
    public string FFProbeExecutable { get; set; } = "ffprobe";
    public int GlobalStreamLimit { get; set; } = 1;
    public int MaxConnectRetry { get; set; } = 20;
    public int MaxConnectRetryTimeMS { get; set; } = 200;
    public List<string> NameRegex { get; set; } = [];
    public string SSLCertPassword { get; set; } = string.Empty;
    public string SSLCertPath { get; set; } = string.Empty;
    public string StreamingClientUserAgent { get; set; } = "VLC/3.0.20-git LibVLC/3.0.20-git";
    public string StreamingProxyType { get; set; } = "StreamMaster";
    public bool VideoStreamAlwaysUseEPGLogo { get; set; } = true;
    public bool ShowClientHostNames { get; set; }
}

public class Setting : BaseSettings
{
    [NoMap]
    public string ServerKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
}