using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class ContentRating : IContentRating
{
    [JsonPropertyName("body")]
    public string Body { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("contentAdvisory")]
    public List<string> ContentAdvisory { get; set; }

    public ContentRating() { }
}
