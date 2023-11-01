using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class SDProgram : ISDProgram
{
    [JsonPropertyName("resourceID")]
    public string ResourceID { get; set; }

    [JsonPropertyName("programID")]
    public string ProgramID { get; set; }

    [JsonPropertyName("titles")]
    public List<Title> Titles { get; set; }

    [JsonPropertyName("descriptions")]
    public IDescriptions Descriptions { get; set; }

    [JsonPropertyName("originalAirDate")]
    public string OriginalAirDate { get; set; }

    [JsonPropertyName("genres")]
    public List<string> Genres { get; set; }

    [JsonPropertyName("episodeTitle150")]
    public string EpisodeTitle150 { get; set; }

    [JsonPropertyName("metadata")]
    public List<ProgramMetadata> Metadata { get; set; }

    [JsonPropertyName("contentRating")]
    public List<ContentRating> ContentRating { get; set; }

    [JsonPropertyName("cast")]
    public List<Cast> Cast { get; set; }

    [JsonPropertyName("crew")]
    public List<Crew> Crew { get; set; }

    [JsonPropertyName("entityType")]
    public string EntityType { get; set; }

    [JsonPropertyName("showType")]
    public string ShowType { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("hasImageArtwork")]
    public bool? HasImageArtwork { get; set; }

    [JsonPropertyName("hasSeriesArtwork")]
    public bool? HasSeriesArtwork { get; set; }

    [JsonPropertyName("hasEpisodeArtwork")]
    public bool? HasEpisodeArtwork { get; set; }

    [JsonPropertyName("keyWords")]
    public IKeyWords KeyWords { get; set; }

    [JsonPropertyName("eventDetails")]
    public IEventDetails EventDetails { get; set; }

    [JsonPropertyName("hasSeasonArtwork")]
    public bool? HasSeasonArtwork { get; set; }

    [JsonPropertyName("officialURL")]
    public string OfficialURL { get; set; }

    [JsonPropertyName("contentAdvisory")]
    public List<string> ContentAdvisory { get; set; }

    [JsonPropertyName("movie")]
    public IMovie Movie { get; set; }

    [JsonPropertyName("awards")]
    public List<Award> Awards { get; set; }

    [JsonPropertyName("hasMovieArtwork")]
    public bool? HasMovieArtwork { get; set; }

    [JsonPropertyName("holiday")]
    public string Holiday { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("audience")]
    public string Audience { get; set; }

}
