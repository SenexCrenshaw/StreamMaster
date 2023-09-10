using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Models;
public class Lineup
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("lineup")]
    public string LineupString { get; set; }

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

    // Default constructor
    public Lineup()
    {
        // Initialization logic, if any, can be added here.
    }

    // If you need additional methods or functionality specific to this class, you can add them here.
}

public class LineUpsResult
{
    public LineUpsResult() { }
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("serverID")]
    public string ServerID { get; set; }

    [JsonPropertyName("datetime")]
    public DateTime Datetime { get; set; }

    [JsonPropertyName("lineups")]
    public List<Lineup> Lineups { get; set; }

    // If you need a constructor, default or with parameters, you can add it here.

    // If you need additional methods or functionality specific to this class, you can add them here.
}
