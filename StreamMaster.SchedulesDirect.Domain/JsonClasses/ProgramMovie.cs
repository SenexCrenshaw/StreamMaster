using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramMovie
    {
        [JsonConverter(typeof(IntConverter))]
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("qualityRating")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramQualityRating>))]
        public List<ProgramQualityRating> QualityRating { get; set; } = new();
    }
}