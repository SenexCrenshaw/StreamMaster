using System.Text.Json.Serialization;

namespace StreamMaster.Ring.API.Entities
{
    public class Owner
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
