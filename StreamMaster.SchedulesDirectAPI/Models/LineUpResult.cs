using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Models;


public class LineUpResult
{
    public LineUpResult() { }

    [JsonPropertyName("map")]
    public List<Map> Map { get; set; }

    [JsonPropertyName("stations")]
    public List<Station> Stations { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }
}

public class Station
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
public class StationId

{
    public StationId(string stationID)
    {
        StationID = stationID;
    }
    [JsonPropertyName("stationID")]
    public string StationID { get; set; }
}

public class Broadcaster
{
    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("postalcode")]
    public string Postalcode { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }
}

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

public class Map
{
    [JsonPropertyName("stationID")]
    public string StationID { get; set; }

    [JsonPropertyName("uhfVhf")]
    public int UhfVhf { get; set; }

    [JsonPropertyName("atscMajor")]
    public int AtscMajor { get; set; }

    [JsonPropertyName("atscMinor")]
    public int AtscMinor { get; set; }
}

public class Metadata
{
    [JsonPropertyName("lineup")]
    public string Lineup { get; set; }

    [JsonPropertyName("modified")]
    public DateTime Modified { get; set; }

    [JsonPropertyName("transport")]
    public string Transport { get; set; }
}

public class StationLogo : Logo
{
    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }
}

public class ScheduleMetadata
{
    [JsonPropertyName("modified")]
    public DateTime Modified { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }
}

public class Program
{
    [JsonPropertyName("programID")]
    public string ProgramID { get; set; }

    [JsonPropertyName("airDateTime")]
    public DateTime AirDateTime { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("audioProperties")]
    public List<string> AudioProperties { get; set; }

    [JsonPropertyName("videoProperties")]
    public List<string> VideoProperties { get; set; }

    [JsonPropertyName("new")]
    public bool? New { get; set; }

    [JsonPropertyName("liveTapeDelay")]
    public string LiveTapeDelay { get; set; }

    [JsonPropertyName("educational")]
    public bool? Educational { get; set; }

    [JsonPropertyName("isPremiereOrFinale")]
    public string IsPremiereOrFinale { get; set; }

    [JsonPropertyName("premiere")]
    public bool? Premiere { get; set; }
}

public class Schedule
{
    [JsonPropertyName("stationID")]
    public string StationID { get; set; }

    [JsonPropertyName("programs")]
    public List<Program> Programs { get; set; }

    [JsonPropertyName("metadata")]
    public ScheduleMetadata Metadata { get; set; }
}
