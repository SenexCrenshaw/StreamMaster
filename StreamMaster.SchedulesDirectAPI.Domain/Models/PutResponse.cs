using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class PutResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; }

    [JsonPropertyName("code")]
    public int? Code { get; set; }

    [JsonPropertyName("serverID")]
    public string ServerID { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("changesRemaining")]
    public int? ChangesRemaining { get; set; }

    [JsonPropertyName("datetime")]
    public DateTime? Datetime { get; set; }
}