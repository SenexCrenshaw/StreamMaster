using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class Crew
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

    public Crew() { }
}
