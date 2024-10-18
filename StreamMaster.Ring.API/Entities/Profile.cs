using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class Profile
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("phone_number")]
        public object PhoneNumber { get; set; }

        [JsonPropertyName("authentication_token")]
        public string AuthenticationToken { get; set; }

        [JsonPropertyName("features")]
        public SessionFeatures Features { get; set; }

        [JsonPropertyName("app_brand")]
        public string AppBrand { get; set; }

        [JsonPropertyName("user_flow")]
        public string UserFlow { get; set; }

        [JsonPropertyName("explorer_program_terms")]
        public string ExplorerProgramTerms { get; set; }
    }
}
