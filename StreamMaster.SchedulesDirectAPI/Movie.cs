using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class Movie
{
    [JsonPropertyName("year")]
    public string Year { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("qualityRating")]
    public List<QualityRating> QualityRating { get; set; }

    public Movie() { }
}
