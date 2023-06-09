using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect;

// Root myDeserializedClass = JsonSerializer.Deserialize<List<Root>>(myJsonResponse);
public record Award(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("awardName")] string AwardName,
    [property: JsonPropertyName("year")] string Year,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("recipient")] string Recipient,
    [property: JsonPropertyName("personId")] string PersonId,
    [property: JsonPropertyName("won")] bool? Won
);

public record Cast(
    [property: JsonPropertyName("billingOrder")] string BillingOrder,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("nameId")] string NameId,
    [property: JsonPropertyName("personId")] string PersonId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("characterName")] string CharacterName
);

public record ContentRating(
    [property: JsonPropertyName("body")] string Body,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("country")] string Country,
    [property: JsonPropertyName("contentAdvisory")] IReadOnlyList<string> ContentAdvisory
);

public record Crew(
    [property: JsonPropertyName("billingOrder")] string BillingOrder,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("nameId")] string NameId,
    [property: JsonPropertyName("personId")] string PersonId,
    [property: JsonPropertyName("name")] string Name
);

public record Description100(
    [property: JsonPropertyName("descriptionLanguage")] string DescriptionLanguage,
    [property: JsonPropertyName("description")] string Description
);

public record Description1000(
    [property: JsonPropertyName("descriptionLanguage")] string DescriptionLanguage,
    [property: JsonPropertyName("description")] string Description
);

public record Descriptions(
    [property: JsonPropertyName("description100")] IReadOnlyList<Description100> Description100,
    [property: JsonPropertyName("description1000")] IReadOnlyList<Description1000> Description1000
);

public record EventDetails(
    [property: JsonPropertyName("venue100")] string Venue100,
    [property: JsonPropertyName("teams")] IReadOnlyList<Team> Teams,
    [property: JsonPropertyName("gameDate")] string GameDate
);

public record Gracenote(
    [property: JsonPropertyName("season")] int Season,
    [property: JsonPropertyName("episode")] int Episode,
    [property: JsonPropertyName("totalEpisodes")] int? TotalEpisodes,
    [property: JsonPropertyName("totalSeasons")] int? TotalSeasons
);

public record KeyWords(
    [property: JsonPropertyName("General")] IReadOnlyList<string> General
);

public record ProgramMetadata(
    [property: JsonPropertyName("Gracenote")] Gracenote Gracenote
);

public record Movie(
    [property: JsonPropertyName("year")] string Year,
    [property: JsonPropertyName("duration")] int Duration,
    [property: JsonPropertyName("qualityRating")] IReadOnlyList<QualityRating> QualityRating
);

public record QualityRating(
    [property: JsonPropertyName("ratingsBody")] string RatingsBody,
    [property: JsonPropertyName("rating")] string Rating,
    [property: JsonPropertyName("minRating")] string MinRating,
    [property: JsonPropertyName("maxRating")] string MaxRating,
    [property: JsonPropertyName("increment")] string Increment
);

public record SDProgram(
    [property: JsonPropertyName("resourceID")] string ResourceID,
    [property: JsonPropertyName("programID")] string ProgramID,
    [property: JsonPropertyName("titles")] IReadOnlyList<Title> Titles,
    [property: JsonPropertyName("descriptions")] Descriptions Descriptions,
    [property: JsonPropertyName("originalAirDate")] string OriginalAirDate,
    [property: JsonPropertyName("genres")] IReadOnlyList<string> Genres,
    [property: JsonPropertyName("episodeTitle150")] string EpisodeTitle150,
    [property: JsonPropertyName("metadata")] IReadOnlyList<ProgramMetadata> Metadata,
    [property: JsonPropertyName("contentRating")] IReadOnlyList<ContentRating> ContentRating,
    [property: JsonPropertyName("cast")] IReadOnlyList<Cast> Cast,
    [property: JsonPropertyName("crew")] IReadOnlyList<Crew> Crew,
    [property: JsonPropertyName("entityType")] string EntityType,
    [property: JsonPropertyName("showType")] string ShowType,
    [property: JsonPropertyName("md5")] string Md5,
    [property: JsonPropertyName("hasImageArtwork")] bool? HasImageArtwork,
    [property: JsonPropertyName("hasSeriesArtwork")] bool? HasSeriesArtwork,
    [property: JsonPropertyName("hasEpisodeArtwork")] bool? HasEpisodeArtwork,
    [property: JsonPropertyName("keyWords")] KeyWords KeyWords,
    [property: JsonPropertyName("eventDetails")] EventDetails EventDetails,
    [property: JsonPropertyName("hasSeasonArtwork")] bool? HasSeasonArtwork,
    [property: JsonPropertyName("officialURL")] string OfficialURL,
    [property: JsonPropertyName("contentAdvisory")] IReadOnlyList<string> ContentAdvisory,
    [property: JsonPropertyName("movie")] Movie Movie,
    [property: JsonPropertyName("awards")] IReadOnlyList<Award> Awards,
    [property: JsonPropertyName("hasMovieArtwork")] bool? HasMovieArtwork,
    [property: JsonPropertyName("holiday")] string Holiday,
    [property: JsonPropertyName("duration")] int? Duration,
    [property: JsonPropertyName("audience")] string Audience
);

public record Team(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("isHome")] bool? IsHome
);

public record Title(
    [property: JsonPropertyName("title120")] string Title120
);
