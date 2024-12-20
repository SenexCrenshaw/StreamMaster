using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class Headend
    {
        public string Id => HeadendId;

        [JsonPropertyName("headend")]
        public string HeadendId { get; set; } = string.Empty;

        [JsonPropertyName("transport")]
        public string Transport { get; set; } = string.Empty;

        [JsonPropertyName("location")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("lineups")]
        //[JsonConverter(typeof(SingleOrListConverter<HeadendLineup>))]
        public List<HeadendLineup> Lineups { get; set; } = [];
    }
}