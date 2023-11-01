using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Station : IStation
{
    [JsonPropertyName("affiliate")]
    public string Affiliate { get; set; }

    [JsonPropertyName("broadcaster")]
    public Broadcaster Broadcaster { get; set; }

    [JsonPropertyName("broadcastLanguage")]
    public List<string> BroadcastLanguage { get; set; }

    [JsonPropertyName("callsign")]
    public string Callsign { get; set; }

    [JsonPropertyName("descriptionLanguage")]
    public List<string> DescriptionLanguage { get; set; }

    [JsonPropertyName("isCommercialFree")]
    public bool? IsCommercialFree { get; set; }

    public string LineUp { get; set; }

    [JsonPropertyName("logo")]
    public Logo Logo { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("stationID")]
    public string StationID { get; set; }

    [JsonPropertyName("stationLogo")]
    public List<StationLogo> StationLogo { get; set; }
}
