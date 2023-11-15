using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Award : IAward
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("awardName")]
    public string AwardName { get; set; }

    [JsonPropertyName("year")]
    public string Year { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("recipient")]
    public string Recipient { get; set; }

    [JsonPropertyName("personId")]
    public string PersonId { get; set; }

    [JsonPropertyName("won")]
    public bool? Won { get; set; }

    public Award() { }
}
