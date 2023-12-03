using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.JsonClasses
{
    public class LineupPreviewChannel
    {
        public int Id { get; set; }
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("callsign")]
        public string Callsign { get; set; }

        [JsonPropertyName("affiliate")]
        public string Affiliate { get; set; }
    }
}