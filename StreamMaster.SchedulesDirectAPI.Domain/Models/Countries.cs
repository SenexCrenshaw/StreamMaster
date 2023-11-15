using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Countries : ICountries
{
    [JsonPropertyName("North America")]
    public List<NorthAmerica> NorthAmerica { get; set; }

    [JsonPropertyName("Europe")]
    public List<Europe> Europe { get; set; }

    [JsonPropertyName("Latin America")]
    public List<LatinAmerica> LatinAmerica { get; set; }

    [JsonPropertyName("Caribbean")]
    public List<Caribbean> Caribbean { get; set; }

    [JsonPropertyName("Oceania")]
    public List<Oceanium> Oceania { get; set; }

    public Countries() { }
}