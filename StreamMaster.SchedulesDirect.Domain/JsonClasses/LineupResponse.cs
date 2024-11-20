using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class LineupResponse : BaseResponse
{
    [JsonPropertyName("lineups")]
    // //[JsonConverter(typeof(SingleOrListConverter<SubscribedLineup>))]
    public List<SubscribedLineup> Lineups { get; set; } = [];
}
