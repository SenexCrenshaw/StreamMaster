using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses;


[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class LineupResponse : BaseResponse
{
    [JsonPropertyName("lineups")]
    // //[JsonConverter(typeof(SingleOrListConverter<SubscribedLineup>))]
    public List<SubscribedLineup> Lineups { get; set; }
}

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class SubscribedLineup
{
    public string Id { get => Lineup; }
    public override string ToString()
    {
        return $"{Name} ({Location})";
    }

    [JsonPropertyName("lineup")]
    public string Lineup { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("transport")]
    public string Transport { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
}