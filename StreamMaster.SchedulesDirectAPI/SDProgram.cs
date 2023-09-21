using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class Award
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("awardName")]
    public string AwardName { get; set; }

    [JsonPropertyName("year")]
    public string Year { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("recipient")]
    public string Recipient { get; set; }

    [JsonPropertyName("personId")]
    public string PersonId { get; set; }

    [JsonPropertyName("won")]
    public bool? Won { get; set; }

    public Award() { }
}

public class Cast
{
    [JsonPropertyName("billingOrder")]
    public string BillingOrder { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("nameId")]
    public string NameId { get; set; }

    [JsonPropertyName("personId")]
    public string PersonId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("characterName")]
    public string CharacterName { get; set; }

    public Cast() { }
}

public class ContentRating
{
    [JsonPropertyName("body")]
    public string Body { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("contentAdvisory")]
    public List<string> ContentAdvisory { get; set; }

    public ContentRating() { }
}

public class Crew
{
    [JsonPropertyName("billingOrder")]
    public string BillingOrder { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("nameId")]
    public string NameId { get; set; }

    [JsonPropertyName("personId")]
    public string PersonId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    public Crew() { }
}

public class Description100
{
    [JsonPropertyName("descriptionLanguage")]
    public string DescriptionLanguage { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    public Description100() { }
}

public class Description1000
{
    [JsonPropertyName("descriptionLanguage")]
    public string DescriptionLanguage { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    public Description1000() { }
}

public class Descriptions
{
    [JsonPropertyName("description100")]
    public List<Description100> Description100 { get; set; }

    [JsonPropertyName("description1000")]
    public List<Description1000> Description1000 { get; set; }

    public Descriptions() { }
}

public class EventDetails
{
    [JsonPropertyName("venue100")]
    public string Venue100 { get; set; }

    [JsonPropertyName("teams")]
    public List<Team> Teams { get; set; }

    [JsonPropertyName("gameDate")]
    public string GameDate { get; set; }

    public EventDetails() { }
}

public class Gracenote
{
    [JsonPropertyName("season")]
    public int Season { get; set; }

    [JsonPropertyName("episode")]
    public int Episode { get; set; }

    [JsonPropertyName("totalEpisodes")]
    public int? TotalEpisodes { get; set; }

    [JsonPropertyName("totalSeasons")]
    public int? TotalSeasons { get; set; }

    public Gracenote() { }
}

public class KeyWords
{
    [JsonPropertyName("General")]
    public List<string> General { get; set; }

    public KeyWords() { }
}

public class ProgramMetadata
{
    [JsonPropertyName("Gracenote")]
    public Gracenote Gracenote { get; set; }

    public ProgramMetadata() { }
}

public class Movie
{
    [JsonPropertyName("year")]
    public string Year { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("qualityRating")]
    public List<QualityRating> QualityRating { get; set; }

    public Movie() { }
}

public class QualityRating
{
    [JsonPropertyName("ratingsBody")]
    public string RatingsBody { get; set; }

    [JsonPropertyName("rating")]
    public string Rating { get; set; }

    [JsonPropertyName("minRating")]
    public string MinRating { get; set; }

    [JsonPropertyName("maxRating")]
    public string MaxRating { get; set; }

    [JsonPropertyName("increment")]
    public string Increment { get; set; }

    public QualityRating() { }
}

public class SDProgram
{
    [JsonPropertyName("resourceID")]
    public string ResourceID { get; set; }

    [JsonPropertyName("programID")]
    public string ProgramID { get; set; }

    [JsonPropertyName("titles")]
    public List<Title> Titles { get; set; }

    [JsonPropertyName("descriptions")]
    public Descriptions Descriptions { get; set; }

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
    public KeyWords KeyWords { get; set; }

    [JsonPropertyName("eventDetails")]
    public EventDetails EventDetails { get; set; }

    [JsonPropertyName("hasSeasonArtwork")]
    public bool? HasSeasonArtwork { get; set; }

    [JsonPropertyName("officialURL")]
    public string OfficialURL { get; set; }

    [JsonPropertyName("contentAdvisory")]
    public List<string> ContentAdvisory { get; set; }

    [JsonPropertyName("movie")]
    public Movie Movie { get; set; }

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

    public SDProgram() { }
}

public class Team
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("isHome")]
    public bool? IsHome { get; set; }

    public Team() { }
}

public class Title
{
    [JsonPropertyName("title120")]
    public string Title120 { get; set; }

    public Title() { }
}