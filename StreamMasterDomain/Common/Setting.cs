using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Common;

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


public class BaseSettings : M3USettings
{
    public string AdminPassword { get; set; } = "";
    public string AdminUserName { get; set; } = "";
    public string DefaultIcon { get; set; } = "images/default.png";
    public string UiFolder { get; set; } = "wwwroot";
    public string UrlBase { get; set; } = "";
    public List<string> LogPerformance { get; set; } = new List<string> { "*.Queries" };
    public string ApiKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    public AuthenticationType AuthenticationMethod { get; set; } = AuthenticationType.None;
    public bool CacheIcons { get; set; } = true;
    public bool CleanURLs { get; set; } = true;
    public string ClientUserAgent { get; set; } = "Mozilla/5.0 (compatible; streammaster/1.0)";
    public string DeviceID { get; set; } = "device1";
    public string DummyRegex { get; set; } = "(no tvg-id)";
    public string FFMpegOptions { get; set; } = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1";
    public bool EnableSSL { get; set; } = false;
    public bool EPGAlwaysUseVideoStreamName { get; set; } = false;
    public string FFMPegExecutable { get; set; } = "ffmpeg";
    public int GlobalStreamLimit { get; set; } = 1;
    public int MaxConnectRetry { get; set; } = 20;
    public int MaxConnectRetryTimeMS { get; set; } = 100;
    public bool OverWriteM3UChannels { get; set; } = false;
    public int PreloadPercentage { get; set; } = 25;
    public int RingBufferSizeMB { get; set; } = 4;
    public string SDCountry { get; set; } = string.Empty;
    public string SDPassword { get; set; } = "";
    public string SDPostalCode { get; set; } = string.Empty;
    public List<StationIdLineUp> SDStationIds { get; set; } = new();
    public List<string> NameRegex { get; set; } = new();
    public string SDUserName { get; set; } = "";
    public string SSLCertPassword { get; set; } = "";
    public string SSLCertPath { get; set; } = "";
    public string StreamingClientUserAgent { get; set; } = "Mozilla/5.0 (compatible; streammaster/1.0)";
    public StreamingProxyTypes StreamingProxyType { get; set; } = StreamingProxyTypes.StreamMaster;
    public bool VideoStreamAlwaysUseEPGLogo { get; set; } = false;
}


public class ProtectedSettings : BaseSettings
{
    [NoMap]
    public string ServerKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");


}

public class Setting : ProtectedSettings, ISetting
{
}