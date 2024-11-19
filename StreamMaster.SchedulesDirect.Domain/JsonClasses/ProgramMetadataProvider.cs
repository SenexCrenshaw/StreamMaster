using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.JsonClasses
{
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
        public string Url { get; set; } = string.Empty;
    }
}