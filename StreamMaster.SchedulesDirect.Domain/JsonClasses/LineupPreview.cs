using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
    public class LineupPreviewChannel
    {
        public int Id { get; set; }
        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("callsign")]
        public string Callsign { get; set; } = string.Empty;

        [JsonPropertyName("affiliate")]
        public string Affiliate { get; set; } = string.Empty;
    }
}