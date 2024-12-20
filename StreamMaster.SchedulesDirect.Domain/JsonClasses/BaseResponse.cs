using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class BaseResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("serverID")]
    public string ServerId { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("datetime")]
    public DateTime Datetime { get; set; } = DateTime.MinValue;
    public bool ShouldSerializeDatetime()
    {
        return Datetime.Ticks > 0;
    }

    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;
}