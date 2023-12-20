using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.JsonClasses
{
    public class Programme : BaseResponse
    {
        [JsonPropertyName("programID")]
        public string ProgramId { get; set; }

        [JsonPropertyName("resourceID"), JsonIgnore]
        public string ResourceId { get; set; }

        [JsonPropertyName("titles")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramTitle>))]
        public List<ProgramTitle> Titles { get; set; }

        [JsonPropertyName("episodeTitle150")]
        public string EpisodeTitle150 { get; set; }

        [JsonPropertyName("descriptions")]
        public ProgramDescriptions Descriptions { get; set; }

        [JsonPropertyName("eventDetails")]
        public ProgramEventDetails EventDetails { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, ProgramMetadataProvider>[] Metadata { get; set; }

        [JsonPropertyName("originalAirDate")]
        public DateTime OriginalAirDate { get; set; }
        public bool ShouldSerializeOriginalAirDate() => OriginalAirDate.Ticks > 0;


        [XmlAttribute(AttributeName = "channel")]
        public string Channel { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        [DefaultValue(0)]
        public int Duration { get; set; }

        [JsonPropertyName("movie")]
        public ProgramMovie Movie { get; set; } = new();

        [JsonPropertyName("genres")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Genres { get; set; }

        [JsonPropertyName("officialURL")]
        public string OfficialUrl { get; set; }

        [JsonPropertyName("contentRating")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramContentRating>))]
        public List<ProgramContentRating> ContentRating { get; set; } = [];

        [JsonPropertyName("contentAdvisory")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] ContentAdvisory { get; set; }

        [JsonPropertyName("cast")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramPerson>))]
        public List<ProgramPerson> Cast { get; set; } = [];

        [JsonPropertyName("crew")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramPerson>))]
        public List<ProgramPerson> Crew { get; set; } = [];

        /// <summary>
        /// program type; one of following values;
        /// Show, Episode, Sports, Movie
        /// </summary>
        [JsonPropertyName("entityType")]
        public string EntityType { get; set; }

        /// <summary>
        /// program subtype; one of following values:
        /// Feature Film, Short Film, TV Movie, Miniseries, Series, Special, Sports event, Sports non-event, Paid Programming, Theatre Event, TBA, Off Air
        /// </summary>
        [JsonPropertyName("showType")]
        public string ShowType { get; set; }

        [JsonPropertyName("episodeImage")]
        public string EpisodeImage { get; set; }

        [JsonPropertyName("hasImageArtwork")]
        public bool HasImageArtwork { get; set; }

        [JsonPropertyName("hasSeriesArtwork")]
        public bool HasSeriesArtwork { get; set; }

        [JsonPropertyName("hasSeasonArtwork")]
        public bool HasSeasonArtwork { get; set; }

        [JsonPropertyName("hasEpisodeArtwork")]
        public bool HasEpisodeArtwork { get; set; }

        [JsonPropertyName("hasMovieArtwork")]
        public bool HasMovieArtwork { get; set; }

        [JsonPropertyName("hasSportsArtwork")]
        public bool HasSportsArtwork { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; }

        public string Name { get; set; }
    }

    public class ProgramTitle
    {
        [JsonPropertyName("title120")]
        public string Title120 { get; set; }

        [JsonPropertyName("titleLanguage")]
        public string TitleLanguage { get; set; }
    }

    public class ProgramEventDetails
    {
        [JsonPropertyName("venue100")]
        public string Venue100 { get; set; }

        [JsonPropertyName("teams")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramEventDetailsTeam>))]
        public List<ProgramEventDetailsTeam> Teams { get; set; } = [];

        [JsonPropertyName("gameDate")]
        public DateTime GameDate { get; set; }
        public bool ShouldSerializeGameDate() => GameDate.Ticks > 0;
    }

    public class ProgramEventDetailsTeam
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("isHome")]
        public bool IsHome { get; set; }
    }


    public class ProgramDescriptions
    {
        [JsonPropertyName("description100")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramDescription>))]
        public List<ProgramDescription> Description100 { get; set; }

        [JsonPropertyName("description1000")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramDescription>))]
        public List<ProgramDescription> Description1000 { get; set; }
    }

    public class ProgramDescription
    {
        [JsonPropertyName("descriptionLanguage")]
        public string DescriptionLanguage { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class ProgramKeyWords
    {
        [JsonPropertyName("Mood")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Mood { get; set; }

        [JsonPropertyName("Time Period")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] TimePeriod { get; set; }

        [JsonPropertyName("Theme")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Theme { get; set; }

        [JsonPropertyName("Character")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Character { get; set; }

        [JsonPropertyName("Setting")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Setting { get; set; }

        [JsonPropertyName("Subject")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Subject { get; set; }

        [JsonPropertyName("General")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] General { get; set; }
    }

    public class ProgramMetadataProvider
    {
        [JsonPropertyName("seriesID")]
        public uint SeriesId { get; set; }

        [JsonPropertyName("episodeID")]
        public int EpisodeId { get; set; }

        [JsonPropertyName("season")]
        public int SeasonNumber { get; set; }

        [JsonPropertyName("episode")]
        public int EpisodeNumber { get; set; }

        [JsonPropertyName("totalEpisodes")]
        public int TotalEpisodes { get; set; }

        [JsonPropertyName("totalSeasons")]
        public int TotalSeasons { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class ProgramContentRating
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("contentAdvisory")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] ContentAdvisory { get; set; }
    }



    public class ProgramMovie
    {
        [JsonConverter(typeof(IntConverter))]
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("qualityRating")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramQualityRating>))]
        public List<ProgramQualityRating> QualityRating { get; set; }
    }

    public class ProgramQualityRating
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
    }

    public class ProgramPerson
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
    }

    public class ProgramRecommendation
    {
        [JsonPropertyName("programID")]
        public string ProgramId { get; set; }

        [JsonPropertyName("title120")]
        public string Title120 { get; set; }
    }

    public class ProgramAward
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("awardName")]
        public string AwardName { get; set; }

        [JsonPropertyName("recipient")]
        public string Recipient { get; set; }

        [JsonPropertyName("personId")]
        public string PersonId { get; set; }

        [JsonPropertyName("won")]
        public bool Won { get; set; }

        [JsonPropertyName("year")]
        public string Year { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}