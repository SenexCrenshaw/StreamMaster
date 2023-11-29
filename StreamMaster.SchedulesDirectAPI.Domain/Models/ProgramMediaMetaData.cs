using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class ProgramMediaMetaData
{
    [JsonPropertyName("programID")]
    public string ProgramID { get; set; }

    [JsonPropertyName("data")]
    public List<ImageData> ImageData { get; set; }
}

public class ImageData
{
    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("ratio")]
    public string Ratio { get; set; }

    [JsonPropertyName("aspect")]
    public string Aspect { get; set; }

    [JsonPropertyName("primary")]
    public string Primary { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("tier")]
    public string Tier { get; set; }

    [JsonPropertyName("lastUpdate")]
    public DateTime LastUpdate { get; set; }
}
