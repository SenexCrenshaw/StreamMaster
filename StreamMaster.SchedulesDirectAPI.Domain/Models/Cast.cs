using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Cast : ICast
{
    [JsonPropertyName("billingOrder")]
    public string BillingOrder { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("nameId")]
    public string NameId { get; set; }

    [JsonPropertyName("personId")]
    public string PersonId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("characterName")]
    public string CharacterName { get; set; }

}
