using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Title : ITitle
{
    [JsonPropertyName("title120")]
    public string Title120 { get; set; }

    public Title() { }
}