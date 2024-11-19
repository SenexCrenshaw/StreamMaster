using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
    public class Programme : BaseResponse
    {
        [JsonPropertyName("programID")]
        public string ProgramId { get; set; } = string.Empty;

        [JsonPropertyName("resourceID"), JsonIgnore]
        public string ResourceId { get; set; } = string.Empty;

        [JsonPropertyName("titles")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramTitle>))]
        public List<ProgramTitle> Titles { get; set; } = [];

        [JsonPropertyName("episodeTitle150")]
        public string EpisodeTitle150 { get; set; } = string.Empty;

        [JsonPropertyName("descriptions")]
        public ProgramDescriptions Descriptions { get; set; } = new();

        [JsonPropertyName("eventDetails")]
        public ProgramEventDetails EventDetails { get; set; } = new();

        [JsonPropertyName("metadata")]
        public List<Dictionary<string, ProgramMetadataProvider>> Metadata { get; set; } = [];

        [JsonPropertyName("originalAirDate")]
        public DateTime OriginalAirDate { get; set; }
        public bool ShouldSerializeOriginalAirDate()
        {
            return OriginalAirDate.Ticks > 0;
        }

        [XmlAttribute(AttributeName = "channel")]
        public string Channel { get; set; } = string.Empty;

        [JsonPropertyName("duration")]
        [DefaultValue(0)]
        public int Duration { get; set; }

        [JsonPropertyName("movie")]
        public ProgramMovie Movie { get; set; } = new();

        [JsonPropertyName("genres")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public string[] Genres { get; set; } = [];

        [JsonPropertyName("officialURL")]
        public string OfficialUrl { get; set; } = string.Empty;

        [JsonPropertyName("contentRating")]
        //[JsonConverter(typeof(SingleOrListConverter<ProgramContentRating>))]
        public List<ProgramContentRating> ContentRating { get; set; } = [];

        [JsonPropertyName("contentAdvisory")]
        ////[JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> ContentAdvisory { get; set; } = [];

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
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// program subtype; one of following values:
        /// Feature Film, Short Film, TV Movie, Miniseries, Series, Special, Sports event, Sports non-event, Paid Programming, Theatre Event, TBA, Off Air
        /// </summary>
        [JsonPropertyName("showType")]
        public string ShowType { get; set; } = string.Empty;

        [JsonPropertyName("episodeImage")]
        public string EpisodeImage { get; set; } = string.Empty;

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
        public string Md5 { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}