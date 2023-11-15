using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Descriptions : IDescriptions
{
    [JsonPropertyName("description100")]
    public List<Description100> Description100 { get; set; }

    [JsonPropertyName("description1000")]
    public List<Description1000> Description1000 { get; set; }

}
