using StreamMasterDomain.Authentication;

namespace StreamMasterDomain.Common;

public class Setting
{        
    public string AdminPassword { get; set; }="";
    public string AdminUserName { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string AppName { get; set; } ="StreamMaster";
    public AuthenticationType AuthenticationMethod { get; set; } = AuthenticationType.None;
    public bool EnableSSL { get; set; } = false;
    public string SSLCertPath { get; set; } = "";
    public string SSLCertPassword { get; set; } = "";
    public bool CacheIcons { get; set; } = true;
    public bool CleanURLs { get; set; } = true;
    public string ClientUserAgent { get; set; } = "Mozilla/5.0 (compatible; streammaster/1.0)";
    public string DatabaseName { get; set; } ="StreamMaster.db";
    public string DefaultIcon { get; set; } = "images/default.png";
    public string DeviceID { get; set; } = "device1";
    public string FFMPegExecutable { get; set; } = "ffmpeg";
    public long FirstFreeNumber { get; set; } =1000;
    public int MaxConnectRetry { get; set; } = 20;
    public int MaxConnectRetryTimeMS { get; set; } = 100;
    public bool OverWriteM3UChannels { get; set; } = false;
    public int RingBufferSizeMB { get; set; } = 4;
    public string SDPassword { get; set; } = "";
    public string SDUserName { get; set; } = "";
    public string ServerKey { get; set; } = "";
    public StreamingProxyTypes StreamingProxyType { get; set; }= StreamingProxyTypes.StreamMaster;
    public string StreamMasterIcon { get; set; } = "images/StreamMaster.png";
    public string UiFolder { get; set; } = "wwwroot";
    public string UrlBase { get; set; } = "";        
}
