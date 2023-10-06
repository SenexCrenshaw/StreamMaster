using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class Title
{
    [JsonPropertyName("title120")]
    public string Title120 { get; set; }

    public Title() { }
}