using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class KeyWords : IKeyWords
{
    [JsonPropertyName("General")]
    public List<string> General { get; set; }

    public KeyWords() { }
}
