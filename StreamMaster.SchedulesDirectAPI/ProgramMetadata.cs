using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class ProgramMetadata
{
    [JsonPropertyName("Gracenote")]
    public Gracenote Gracenote { get; set; }

    public ProgramMetadata() { }
}
