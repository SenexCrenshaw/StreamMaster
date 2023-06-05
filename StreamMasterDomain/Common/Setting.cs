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
            AppName = "StreamMaster";
            BaseHostURL = "http://127.0.0.1:7095/";
            CacheIcons = true;
            CleanURLs = true;
            DatabaseName = "StreamMaster.db";
            DefaultIcon = "images/default.png";
            DeviceID = "device1";
            FirstFreeNumber = 1000;
            MaxConnectRetry = 20;
            MaxConnectRetryTimeMS = 100;
            RingBufferSizeMB = 4;
            SourceBufferPreBufferPercentage = 20;
            StreamingProxyType = StreamingProxyTypes.StreamMaster;
            StreamMasterIcon = "images/StreamMaster.png";
            FFMPegExecutable = "ffmpeg";
            SDUserName = "";
            SDPassword = "";
        }

        /// <summary>
        /// The name of the StreamMaster application.
        /// </summary>
        public string AppName { get; set; }

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

        /// <summary>
        /// The size of the ring buffer used by the StreamMaster application in MB.
        /// </summary>
        public int RingBufferSizeMB { get; set; }

        /// <summary>
        /// The password used to authenticate with the SD card used by the
        /// StreamMaster application.
        /// </summary>
        public string SDPassword { get; set; }

        /// <summary>
        /// The username used to authenticate with the SD card used by the
        /// StreamMaster application.
        /// </summary>
        public string SDUserName { get; set; }

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

        ///// <summary>
        ///// The version of the StreamMaster application.
        ///// </summary>
        //public string Version { get; set; }
    }
}
