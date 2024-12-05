using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramQualityRating
    {
        [JsonPropertyName("ratingsBody")]
        public string RatingsBody { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("minRating")]
        public string MinRating { get; set; } = string.Empty;

        [JsonPropertyName("maxRating")]
        public string MaxRating { get; set; } = string.Empty;

        [JsonPropertyName("increment")]
        public string Increment { get; set; } = string.Empty;
    }
}