using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Description1000 : IDescription1000
{
    [JsonPropertyName("descriptionLanguage")]
    public string DescriptionLanguage { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    public Description1000() { }
}
