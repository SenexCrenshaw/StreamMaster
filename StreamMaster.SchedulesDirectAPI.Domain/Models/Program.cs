using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Program
{
    [JsonPropertyName("programID")]
    public string ProgramID { get; set; }

    [JsonPropertyName("airDateTime")]
    public DateTime AirDateTime { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("audioProperties")]
    public List<string> AudioProperties { get; set; }

    [JsonPropertyName("ratings")]
    public List<Rating> Ratings { get; set; }

    [JsonPropertyName("new")]
    public bool? New { get; set; }

    [JsonPropertyName("isPremiereOrFinale")]
    public string IsPremiereOrFinale { get; set; }

    [JsonPropertyName("multipart")]
    public Multipart Multipart { get; set; }

    [JsonPropertyName("SAPLanguage")]
    public string SAPLanguage { get; set; }

    [JsonPropertyName("premiere")]
    public bool? Premiere { get; set; }

    [JsonPropertyName("liveTapeDelay")]
    public string LiveTapeDelay { get; set; }

    [JsonPropertyName("repeat")]
    public bool? Repeat { get; set; }

    [JsonPropertyName("subtitledLanguage")]
    public string SubtitledLanguage { get; set; }

    [JsonPropertyName("videoProperties")]
    public List<string> VideoProperties { get; set; }

    [JsonPropertyName("educational")]
    public bool? Educational { get; set; }

    [JsonPropertyName("joinedInProgress")]
    public bool? JoinedInProgress { get; set; }
}
