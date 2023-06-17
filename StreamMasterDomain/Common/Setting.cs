using StreamMasterDomain.Authentication;

/// <summary>
/// The Setting class represents the settings of the StreamMaster application.
/// It contains properties that specify various settings such as the application
/// name, database name, default icon, device ID, and more.
/// </summary>
namespace StreamMasterDomain.Common
{
    public class Setting
    {
        /// <summary>
        /// Default constructor that initializes the properties with their
        /// default values.
        /// </summary>
        public Setting()
        {
            AdminPassword = "";
            AdminUserName = "";
            ApiKey = "";
            ServerKey = "";
            APIPassword = "";
            APIUserName = "";
            AppName = "StreamMaster";
            AuthenticationMethod = AuthenticationType.None;
            AuthTest = true;
            BaseHostURL = "http://127.0.0.1:7095/";
            CacheIcons = true;
            CleanURLs = true;
            DatabaseName = "StreamMaster.db";
            DefaultIcon = "images/default.png";
            DeviceID = "device1";
            FFMPegExecutable = "ffmpeg";
            FirstFreeNumber = 1000;
            MaxConnectRetry = 20;
            MaxConnectRetryTimeMS = 100;
            OverWriteM3UChannels = false;
            RingBufferSizeMB = 4;
            SDPassword = "";
            SDUserName = "";
            SourceBufferPreBufferPercentage = 20;
            StreamingProxyType = StreamingProxyTypes.StreamMaster;
            StreamMasterIcon = "images/StreamMaster.png";
            UiFolder = "wwwroot";
            UrlBase = "";
        }

        public string AdminPassword { get; set; }
        public string AdminUserName { get; set; }
        public string ApiKey { get; set; }
        public string APIPassword { get; set; }
        public string APIUserName { get; set; }

        /// <summary>
        /// The name of the StreamMaster application.
        /// </summary>
        public string AppName { get; set; }

        public AuthenticationType AuthenticationMethod { get; set; }
        public bool AuthTest { get; set; }

        /// <summary>
        /// The base URL of the StreamMaster application.
        /// </summary>
        public string BaseHostURL { get; set; }

        /// <summary>
        /// A boolean value indicating whether icons should be cached or not.
        /// </summary>
        public bool CacheIcons { get; set; }

        public bool CleanURLs { get; set; }

        /// <summary>
        /// The name of the database used by the StreamMaster application.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// The path of the default icon used by the StreamMaster application.
        /// </summary>
        public string DefaultIcon { get; set; }

        /// <summary>
        /// The unique identifier of the device running the StreamMaster application.
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// The path of the FFmpeg executable used by the StreamMaster application.
        /// </summary>
        public string FFMPegExecutable { get; set; }

        /// <summary>
        /// The first available number to be assigned to a stream in the
        /// StreamMaster application.
        /// </summary>
        public long FirstFreeNumber { get; set; }

        /// <summary>
        /// The maximum number of missed errors allowed for a stream in the
        /// StreamMaster application.
        /// </summary>
        public int MaxConnectRetry { get; set; }

        public int MaxConnectRetryTimeMS { get; set; }
        public bool OverWriteM3UChannels { get; set; }

        /// <summary>
        /// The size of the ring buffer used by the StreamMaster application in MB.
        /// </summary>
        public int RingBufferSizeMB { get; set; }

        /// <summary>
        /// The password used to authenticate with the SD card used by the
        /// StreamMaster application.
        /// </summary>
        public string SDPassword { get; set; }

        //public List<string> SafeNetworks { get; set; }
        //{
        //    get
        //    {
        //        List<string> modifiedList = new List<string>(_safeNetworks);
        //        modifiedList.Insert(0, "test");
        //        return= _safeNetworks.Select(s => "test" + s).ToList();
        //    }
        //    set
        //    {
        //        _safeNetworks = value;
        //    }
        //}
        /// <summary>
        /// The username used to authenticate with the SD card used by the
        /// StreamMaster application.
        /// </summary>
        public string SDUserName { get; set; }

        public string ServerKey { get; set; }

        /// <summary>
        /// The percentage of the source buffer to be pre-buffered by the
        /// StreamMaster application.
        /// </summary>
        public int SourceBufferPreBufferPercentage { get; set; }

        /// <summary>
        /// The type of the streaming proxy used by the StreamMaster application.
        /// </summary>
        public StreamingProxyTypes StreamingProxyType { get; set; }

        /// <summary>
        /// The path of the StreamMaster icon used by the StreamMaster application.
        /// </summary>
        public string StreamMasterIcon { get; set; }

        public string UiFolder { get; set; }
        public string UrlBase { get; set; }
        private List<string> _safeNetworks { get; set; }
        ///// <summary>
        ///// The version of the StreamMaster application.
        ///// </summary>
        //public string Version { get; set; }
    }
}
