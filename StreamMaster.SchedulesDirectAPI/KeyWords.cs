using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class KeyWords
{
    [JsonPropertyName("General")]
    public List<string> General { get; set; }

    public KeyWords() { }
}
