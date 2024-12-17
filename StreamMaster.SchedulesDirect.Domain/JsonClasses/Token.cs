using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class TokenRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}