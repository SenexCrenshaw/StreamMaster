using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class Description1000
{
    [JsonPropertyName("descriptionLanguage")]
    public string DescriptionLanguage { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    public Description1000() { }
}
