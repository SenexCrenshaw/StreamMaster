using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;
public record Lineup(
     [property: JsonPropertyName("id")] string Id,
      [property: JsonPropertyName("lineup")] string LineupString,
      [property: JsonPropertyName("name")] string Name,
      [property: JsonPropertyName("transport")] string Transport,
      [property: JsonPropertyName("location")] string Location,
      [property: JsonPropertyName("uri")] string Uri,
      [property: JsonPropertyName("isDeleted")] bool IsDeleted
  );

public record LineUpsResult(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("serverID")] string ServerID,
    [property: JsonPropertyName("datetime")] DateTime Datetime,
    [property: JsonPropertyName("lineups")] IReadOnlyList<Lineup> Lineups
);
