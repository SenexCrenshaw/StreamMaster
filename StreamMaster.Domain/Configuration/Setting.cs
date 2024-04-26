using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Configuration;

public class BaseSettings
{
    public bool M3UFieldGroupTitle { get; set; } = true;
    public bool M3UIgnoreEmptyEPGID { get; set; } = true;
    public bool M3UUseChnoForId { get; set; } = true;
    public bool M3UUseCUIDForChannelID { get; set; } = false;
    public bool M3UStationId { get; set; } = false;
    public bool BackupEnabled { get; set; } = true;
    public int BackupVersionsToKeep { get; set; } = 18;
    public int BackupInterval { get; set; } = 4;
    public bool PrettyEPG { get; set; } = false;
    public int MaxLogFiles { get; set; } = 10;
    public int MaxLogFileSizeMB { get; set; } = 1;
    public bool EnablePrometheus { get; set; } = false;
    public int MaxStreamReStart { get; set; } = 3;
    public int MaxConcurrentDownloads { get; set; } = 8;
    public int ExpectedServiceCount { get; set; } = 20;
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
    public string FFMpegOptions { get; set; } = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1";
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
    public StreamingProxyTypes StreamingProxyType { get; set; } = StreamingProxyTypes.StreamMaster;
    public bool VideoStreamAlwaysUseEPGLogo { get; set; } = true;
    public bool ShowClientHostNames { get; set; }
}

public class Setting : BaseSettings
{
    [NoMap]
    public string ServerKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
}

//public class OldSetting : BaseSettings
//{
//    public Setting ConvertToSetting()
//    {
//        Setting setting = new()
//        {
//            ServerKey = ServerKey,
//            M3UFieldGroupTitle = M3UFieldGroupTitle,
//            M3UIgnoreEmptyEPGID = M3UIgnoreEmptyEPGID,
//            M3UUseChnoForId = M3UUseChnoForId,
//            M3UUseCUIDForChannelID = M3UUseCUIDForChannelID,
//            M3UStationId = M3UStationId,
//            BackupEnabled = BackupEnabled,
//            BackupVersionsToKeep = BackupVersionsToKeep,
//            BackupInterval = BackupInterval,
//            PrettyEPG = PrettyEPG,
//            MaxLogFiles = MaxLogFiles,
//            MaxLogFileSizeMB = MaxLogFileSizeMB,
//            EnablePrometheus = EnablePrometheus,
//            MaxStreamReStart = MaxStreamReStart,
//            MaxConcurrentDownloads = MaxConcurrentDownloads,
//            ExpectedServiceCount = ExpectedServiceCount,
//            AdminPassword = AdminPassword,
//            AdminUserName = AdminUserName,
//            DefaultIcon = DefaultIcon,
//            UiFolder = UiFolder,
//            UrlBase = UrlBase,
//            LogPerformance = new List<string>(LogPerformance),
//            ApiKey = ApiKey,
//            AuthenticationMethod = AuthenticationMethod,
//            CacheIcons = CacheIcons,
//            CleanURLs = CleanURLs,
//            ClientUserAgent = ClientUserAgent,
//            DeviceID = DeviceID,
//            DummyRegex = DummyRegex,
//            FFMpegOptions = FFMpegOptions,
//            EnableSSL = EnableSSL,
//            FFMPegExecutable = FFMPegExecutable,
//            FFProbeExecutable = FFProbeExecutable,
//            GlobalStreamLimit = GlobalStreamLimit,
//            MaxConnectRetry = MaxConnectRetry,
//            MaxConnectRetryTimeMS = MaxConnectRetryTimeMS,
//            NameRegex = new List<string>(NameRegex),
//            SSLCertPassword = SSLCertPassword,
//            SSLCertPath = SSLCertPath,
//            StreamingClientUserAgent = StreamingClientUserAgent,
//            StreamingProxyType = StreamingProxyType,
//            VideoStreamAlwaysUseEPGLogo = VideoStreamAlwaysUseEPGLogo,
//            ShowClientHostNames = ShowClientHostNames
//        };

//        return setting;
//    }

//    [NoMap]
//    public string ServerKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

//    public SDSettings? SDSettings { get; set; }

//}

public class FFMPEGProfiles
{
    public Dictionary<string, FFMPEGProfile> Profiles { get; set; } = [];
}

public class FFMPEGProfilesDto
{
    public Dictionary<string, FFMPEGProfile> Profiles { get; set; } = [];
}