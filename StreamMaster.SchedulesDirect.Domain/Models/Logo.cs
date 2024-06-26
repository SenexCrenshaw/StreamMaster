using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class Logo
{
    [JsonPropertyName("URL")]
    public string URL { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }
}
