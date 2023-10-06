using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Models;

public class Program
{
    [JsonPropertyName("programID")]
    public string ProgramID { get; set; }

    [JsonPropertyName("airDateTime")]
    public DateTime AirDateTime { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("audioProperties")]
    public List<string> AudioProperties { get; set; }

    [JsonPropertyName("videoProperties")]
    public List<string> VideoProperties { get; set; }

    [JsonPropertyName("new")]
    public bool? New { get; set; }

    [JsonPropertyName("liveTapeDelay")]
    public string LiveTapeDelay { get; set; }

    [JsonPropertyName("educational")]
    public bool? Educational { get; set; }

    [JsonPropertyName("isPremiereOrFinale")]
    public string IsPremiereOrFinale { get; set; }

    [JsonPropertyName("premiere")]
    public bool? Premiere { get; set; }
}
