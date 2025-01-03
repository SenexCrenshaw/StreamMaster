using System.Text.Json.Serialization;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class EPGJsonCache
{
    private string? jsonEntry;

    [JsonPropertyName("jsonEntry")]
    public string? JsonEntry
    {
        get => jsonEntry; set
        {
            if (value != null) { SetCurrent(); jsonEntry = value; }
        }
    }

    //[JsonIgnore]
    //[IgnoreMember]
    public bool Current => LastUpdated.AddDays(BuildInfo.SDCacheDurationDays) > SMDT.UtcNow;

    public DateTime LastUpdated { get; set; } = SMDT.UtcNow;

    public void SetCurrent()
    {
        LastUpdated = SMDT.UtcNow;
    }
}