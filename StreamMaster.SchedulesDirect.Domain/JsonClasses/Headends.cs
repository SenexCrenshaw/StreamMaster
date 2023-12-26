using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class Headend
    {
        public string Id => HeadendId;

        [JsonPropertyName("headend")]
        public string HeadendId { get; set; }

        [JsonPropertyName("transport")]
        public string Transport { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("lineups")]
        //[JsonConverter(typeof(SingleOrListConverter<HeadendLineup>))]
        public List<HeadendLineup> Lineups { get; set; } = [];
    }

    public class HeadendLineup
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lineup")]
        public string Lineup { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}