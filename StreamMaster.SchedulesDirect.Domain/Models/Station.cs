using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class Station
{


    [JsonPropertyName("affiliate")]
    public string? Affiliate { get; set; }

    [JsonPropertyName("broadcaster")]
    public Broadcaster? Broadcaster { get; set; }

    [JsonPropertyName("broadcastLanguage")]
    public List<string>? BroadcastLanguage { get; set; }

    [JsonPropertyName("callsign")]
    public string? Callsign { get; set; }

    [JsonPropertyName("descriptionLanguage")]
    public List<string>? DescriptionLanguage { get; set; }

    [JsonPropertyName("isCommercialFree")]
    public bool? IsCommercialFree { get; set; }

    public string Lineup { get; set; } = string.Empty;

    [JsonPropertyName("logo")]
    public Logo? Logo { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("stationID")]
    public string? StationId { get; set; }

    public string? Country { get; set; }
    public string? PostalCode { get; set; }


    [JsonPropertyName("stationLogo")]
    //[JsonConverter(typeof(SingleOrListConverter<StationImage>))]
    public List<Logo> StationLogos { get; set; } = [];

}
