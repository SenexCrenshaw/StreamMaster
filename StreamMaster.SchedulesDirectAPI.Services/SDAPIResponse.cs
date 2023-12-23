using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Services;

internal class SDAPIResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("datetime")]
    public DateTime Datetime { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("response")]
    public string Response { get; set; }

    [JsonPropertyName("serverID")]
    public string ServerID { get; set; }
}
