namespace StreamMasterDomain.Common
{
    public interface ISetting
    {
        string AdminPassword { get; set; }
        string AdminUserName { get; set; }
        string ApiKey { get; set; }
        AuthenticationType AuthenticationMethod { get; set; }
        bool CacheIcons { get; set; }
        bool CleanURLs { get; set; }
        string ClientUserAgent { get; set; }
        string DefaultIcon { get; set; }
        string DeviceID { get; set; }
        string DummyRegex { get; set; }
        bool EnableSSL { get; set; }
        bool EPGAlwaysUseVideoStreamName { get; set; }
        string FFMPegExecutable { get; set; }
        int GlobalStreamLimit { get; set; }
        bool M3UFieldChannelId { get; set; }
        bool M3UFieldChannelNumber { get; set; }
        bool M3UFieldCUID { get; set; }
        bool M3UFieldGroupTitle { get; set; }
        bool M3UFieldTvgChno { get; set; }
        bool M3UFieldTvgId { get; set; }
        bool M3UFieldTvgLogo { get; set; }
        bool M3UFieldTvgName { get; set; }
        bool M3UIgnoreEmptyEPGID { get; set; }
        int MaxConnectRetry { get; set; }
        int MaxConnectRetryTimeMS { get; set; }
        List<string> NameRegex { get; set; }
        bool OverWriteM3UChannels { get; set; }
        int PreloadPercentage { get; set; }
        int RingBufferSizeMB { get; set; }
        string SDCountry { get; set; }
        string SDPassword { get; set; }
        string SDPostalCode { get; set; }
        List<string> SDStationIds { get; set; }
        string SDUserName { get; set; }
        string ServerKey { get; set; }
        string SSLCertPassword { get; set; }
        string SSLCertPath { get; set; }
        string StreamingClientUserAgent { get; set; }
        StreamingProxyTypes StreamingProxyType { get; set; }
        string StreamMasterIcon { get; set; }
        string UiFolder { get; set; }
        string UrlBase { get; set; }
        bool VideoStreamAlwaysUseEPGLogo { get; set; }
    }
}