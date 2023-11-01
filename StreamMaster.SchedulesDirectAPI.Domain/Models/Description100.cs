using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Description100 : IDescription100
{
    [JsonPropertyName("descriptionLanguage")]
    public string DescriptionLanguage { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    public Description100() { }
}
