using MessagePack;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class EPGJsonCache
{
    [JsonPropertyName("jsonEntry")]
    public string? JsonEntry { get; set; }

    [JsonPropertyName("images")]
    public string? Images { get; set; }

    [JsonIgnore]
    [IgnoreMember]
    public bool Current { get; set; }
}

