using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class ProgramPerson
    {
        [JsonPropertyName("billingOrder")]
        public string BillingOrder { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("nameId")]
        public string NameId { get; set; } = string.Empty;

        [JsonPropertyName("personId")]
        public string PersonId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("characterName")]
        public string CharacterName { get; set; } = string.Empty;
    }
}