using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class StatusAccount
    {
        [JsonPropertyName("expires")]
        public DateTime Expires { get; set; } = DateTime.MinValue;

        [JsonPropertyName("messages")]
        //[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Messages { get; set; } = [];

        [JsonPropertyName("maxLineups")]
        public int MaxLineups { get; set; }
    }
}