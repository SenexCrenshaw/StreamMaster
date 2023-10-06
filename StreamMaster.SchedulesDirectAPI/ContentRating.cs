using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class ContentRating
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
