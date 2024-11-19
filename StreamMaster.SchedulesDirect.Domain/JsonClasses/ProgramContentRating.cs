using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramContentRating
    {
        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("contentAdvisory")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> ContentAdvisory { get; set; } = new();
    }
}