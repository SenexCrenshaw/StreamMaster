using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class ProgramMetadata : IProgramMetadata
{
    [JsonPropertyName("Gracenote")]
    public Gracenote Gracenote { get; set; }

}
