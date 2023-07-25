namespace StreamMasterDomain.Common;

public class Setting
{
    public string AdminPassword { get; set; } = "";

    public string AdminUserName { get; set; } = "";

    public string ApiKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

    public AuthenticationType AuthenticationMethod { get; set; } = AuthenticationType.None;
    public bool CacheIcons { get; set; } = true;
    public bool CleanURLs { get; set; } = true;

    public List<string> SDStationIds { get; set; } = new();
    public string SDCountry { get; set; } = string.Empty;
    public string SDPostalCode { get; set; } = string.Empty;
    public string ClientUserAgent { get; set; } = "Mozilla/5.0 (compatible; streammaster/1.0)";

    public string DefaultIcon { get; set; } = "images/default.png";

    public string DeviceID { get; set; } = "device1";
    public string DummyRegex { get; set; } = "(no tvg-id)";
    public bool EnableSSL { get; set; } = false;

    public string FFMPegExecutable { get; set; } = "ffmpeg";

    public int FirstFreeNumber { get; set; } = 1;

    public int GlobalStreamLimit { get; set; } = 1;
    public bool M3UFieldChannelId { get; set; } = true;

    public bool M3UFieldChannelNumber { get; set; } = true;

    public bool M3UFieldCUID { get; set; } = true;

    public bool M3UFieldGroupTitle { get; set; } = true;

    public bool M3UFieldTvgChno { get; set; } = true;
    public bool M3UFieldTvgId { get; set; } = true;
    public bool M3UFieldTvgLogo { get; set; } = true;
    public bool M3UFieldTvgName { get; set; } = true;
    public bool M3UIgnoreEmptyEPGID { get; set; } = true;
    public int MaxConnectRetry { get; set; } = 20;
    public int MaxConnectRetryTimeMS { get; set; } = 100;
    public bool OverWriteM3UChannels { get; set; } = false;
    public int PreloadPercentage { get; set; } = 25;
    public int RingBufferSizeMB { get; set; } = 4;
    public string SDPassword { get; set; } = "";
    public string SDUserName { get; set; } = "";
    public string ServerKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    public string SSLCertPassword { get; set; } = "";
    public string SSLCertPath { get; set; } = "";
    public string StreamingClientUserAgent { get; set; } = "Mozilla/5.0 (compatible; streammaster/1.0)";

    public StreamingProxyTypes StreamingProxyType { get; set; } = StreamingProxyTypes.StreamMaster;

    public string StreamMasterIcon { get; set; } = "images/StreamMaster.png";

    public string UiFolder { get; set; } = "wwwroot";

    public string UrlBase { get; set; } = "";
}
