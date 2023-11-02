using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterDomain.Attributes;

namespace StreamMasterDomain.Common;
[RequireAll]
public class M3USettings
{
    public bool M3UFieldChannelId { get; set; } = true;
    public bool M3UFieldChannelNumber { get; set; } = true;
    public bool M3UFieldCUID { get; set; } = true;
    public bool M3UFieldGroupTitle { get; set; } = true;
    public bool M3UFieldTvgChno { get; set; } = true;
    public bool M3UFieldTvgId { get; set; } = true;
    public bool M3UFieldTvgLogo { get; set; } = true;
    public bool M3UFieldTvgName { get; set; } = true;
    public bool M3UIgnoreEmptyEPGID { get; set; } = true;
}

public class SDSettings : M3USettings
{
    public int SDEPGDays { get; set; } = 1;
    public int SDMaxRatings { get; set; } = 2;
    public bool SDEnabled { get; set; }
    public string SDUserName { get; set; } = string.Empty;
    public string SDCountry { get; set; } = string.Empty;
    public string SDPassword { get; set; } = string.Empty;
    public string SDPostalCode { get; set; } = string.Empty;
    public List<StationIdLineUp> SDStationIds { get; set; } = new();
}

public class BaseSettings : SDSettings
{
    public string AdminPassword { get; set; } = string.Empty;
    public string AdminUserName { get; set; } = string.Empty;
    public string DefaultIcon { get; set; } = "images/default.png";
    public string UiFolder { get; set; } = "wwwroot";
    public string UrlBase { get; set; } = string.Empty;
    public List<string> LogPerformance { get; set; } = new List<string> { "*.Queries" };
    public string ApiKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    public AuthenticationType AuthenticationMethod { get; set; } = AuthenticationType.None;
    public bool CacheIcons { get; set; } = true;
    public bool CleanURLs { get; set; } = true;
    public string ClientUserAgent { get; set; } = "Mozilla/5.0 (compatible; streammaster/1.0)";
    public string DeviceID { get; set; } = "device1";
    public string DummyRegex { get; set; } = "(no tvg-id)";
    public string FFMpegOptions { get; set; } = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1";
    public bool EnableSSL { get; set; }
    public bool EPGAlwaysUseVideoStreamName { get; set; }
    public string FFMPegExecutable { get; set; } = "ffmpeg";
    public int GlobalStreamLimit { get; set; } = 1;
    public int MaxConnectRetry { get; set; } = 20;
    public int MaxConnectRetryTimeMS { get; set; } = 100;
    public bool OverWriteM3UChannels { get; set; }
    public int PreloadPercentage { get; set; } = 25;
    public int RingBufferSizeMB { get; set; } = 4;

    public List<string> NameRegex { get; set; } = new();

    public string SSLCertPassword { get; set; } = string.Empty;
    public string SSLCertPath { get; set; } = string.Empty;
    public string StreamingClientUserAgent { get; set; } = "Mozilla/5.0 (compatible; streammaster/1.0)";
    public StreamingProxyTypes StreamingProxyType { get; set; } = StreamingProxyTypes.StreamMaster;
    public bool VideoStreamAlwaysUseEPGLogo { get; set; }

    public bool ShowClientHostNames { get; set; }
}

public class ProtectedSettings : BaseSettings
{
    [NoMap]
    public string ServerKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
}

public class Setting : ProtectedSettings, ISetting
{
}