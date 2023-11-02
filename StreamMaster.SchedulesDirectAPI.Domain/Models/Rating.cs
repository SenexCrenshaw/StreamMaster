using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Rating
{
    [JsonPropertyName("body")]
    public string Body { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("subRating")]
    public string SubRating { get; set; }
}
