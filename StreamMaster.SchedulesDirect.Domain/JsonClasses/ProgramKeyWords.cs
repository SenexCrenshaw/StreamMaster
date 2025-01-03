using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramKeyWords
    {
        [JsonPropertyName("Mood")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Mood { get; set; } = [];

        [JsonPropertyName("Time Period")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> TimePeriod { get; set; } = [];

        [JsonPropertyName("Theme")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Theme { get; set; } = [];

        [JsonPropertyName("Character")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Character { get; set; } = [];

        [JsonPropertyName("Setting")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Setting { get; set; } = [];

        [JsonPropertyName("Subject")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Subject { get; set; } = [];

        [JsonPropertyName("General")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> General { get; set; } = [];
    }
}