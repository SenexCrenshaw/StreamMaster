using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace StreamMaster.Ring.API.Entities
{
    /// <summary>
    /// Contains a collection of Ring devices
    /// </summary>
    public class Devices
    {
        /// <summary>
        /// All Ring doorbots
        /// </summary>
        [JsonPropertyName("doorbots")]
        public List<Doorbot> Doorbots { get; set; }

        /// <summary>
        /// All Authorized Ring doorbots
        /// </summary>
        [JsonPropertyName("authorized_doorbots")]
        public List<Doorbot> AuthorizedDoorbots { get; set; }

        /// <summary>
        /// All Ring chimes
        /// </summary>
        [JsonPropertyName("chimes")]
        public List<Chime> Chimes { get; set; }

        /// <summary>
        /// All Ring stickup cameras
        /// </summary>
        [JsonPropertyName("stickup_cams")]
        public List<StickupCam> StickupCams { get; set; }
    }
}
