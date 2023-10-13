using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class Description100
{
    [JsonPropertyName("descriptionLanguage")]
    public string DescriptionLanguage { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    public Description100() { }
}
