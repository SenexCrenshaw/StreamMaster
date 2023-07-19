using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public record StationId(
        [property: JsonPropertyName("stationID")] string StationID
    );

public record Broadcaster(
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("postalcode")] string Postalcode,
    [property: JsonPropertyName("country")] string Country
);

public record Logo(
    [property: JsonPropertyName("URL")] string URL,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("md5")] string Md5
);

public record Map(
    [property: JsonPropertyName("stationID")] string StationID,
    [property: JsonPropertyName("uhfVhf")] int UhfVhf,
    [property: JsonPropertyName("atscMajor")] int AtscMajor,
    [property: JsonPropertyName("atscMinor")] int AtscMinor
);

public record Metadata(
    [property: JsonPropertyName("lineup")] string Lineup,
    [property: JsonPropertyName("modified")] DateTime Modified,
    [property: JsonPropertyName("transport")] string Transport
);

public record LineUpResult(
    [property: JsonPropertyName("map")] IReadOnlyList<Map> Map,
    [property: JsonPropertyName("stations")] IReadOnlyList<Station> Stations,
    [property: JsonPropertyName("metadata")] Metadata Metadata
);

public record Station(
    [property: JsonPropertyName("stationID")] string StationID,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("callsign")] string Callsign,
    [property: JsonPropertyName("affiliate")] string Affiliate,
    [property: JsonPropertyName("broadcastLanguage")] IReadOnlyList<string> BroadcastLanguage,
    [property: JsonPropertyName("descriptionLanguage")] IReadOnlyList<string> DescriptionLanguage,
    [property: JsonPropertyName("broadcaster")] Broadcaster Broadcaster,
    [property: JsonPropertyName("stationLogo")] IReadOnlyList<StationLogo> StationLogo,
    [property: JsonPropertyName("logo")] Logo Logo,
    [property: JsonPropertyName("isCommercialFree")] bool? IsCommercialFree
);

public record StationLogo(
    [property: JsonPropertyName("URL")] string URL,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("md5")] string Md5,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("category")] string Category
);

public record ScheduleMetadata(
    [property: JsonPropertyName("modified")] DateTime Modified,
    [property: JsonPropertyName("md5")] string Md5,
    [property: JsonPropertyName("startDate")] string StartDate
);

public record Program(
    [property: JsonPropertyName("programID")] string ProgramID,
    [property: JsonPropertyName("airDateTime")] DateTime AirDateTime,
    [property: JsonPropertyName("duration")] int Duration,
    [property: JsonPropertyName("md5")] string Md5,
    [property: JsonPropertyName("audioProperties")] IReadOnlyList<string> AudioProperties,
    [property: JsonPropertyName("videoProperties")] IReadOnlyList<string> VideoProperties,
    [property: JsonPropertyName("new")] bool? New,
    [property: JsonPropertyName("liveTapeDelay")] string LiveTapeDelay,
    [property: JsonPropertyName("educational")] bool? Educational,
    [property: JsonPropertyName("isPremiereOrFinale")] string IsPremiereOrFinale,
    [property: JsonPropertyName("premiere")] bool? Premiere
);

public record Schedule(
    [property: JsonPropertyName("stationID")] string StationID,
    [property: JsonPropertyName("programs")] IReadOnlyList<Program> Programs,
    [property: JsonPropertyName("metadata")] ScheduleMetadata Metadata
);
