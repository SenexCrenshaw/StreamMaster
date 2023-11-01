using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Movie : IMovie
{
    [JsonPropertyName("year")]
    public string Year { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("qualityRating")]
    public List<QualityRating> QualityRating { get; set; }

}
