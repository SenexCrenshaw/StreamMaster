using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class DeviceStatus
    {
        /// <summary>
        /// Contains the status of a Ring device
        /// </summary>
        public partial class Status
        {
            [JsonPropertyName("seconds_remaining")]
            public long? SecondsRemaining { get; set; }
        }
    }
}
