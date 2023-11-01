namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ISDProgram
    {
        string Audience { get; set; }
        List<Award> Awards { get; set; }
        List<Cast> Cast { get; set; }
        List<string> ContentAdvisory { get; set; }
        List<ContentRating> ContentRating { get; set; }
        List<Crew> Crew { get; set; }
        Descriptions Descriptions { get; set; }
        int? Duration { get; set; }
        string EntityType { get; set; }
        string EpisodeTitle150 { get; set; }
        EventDetails EventDetails { get; set; }
        List<string> Genres { get; set; }
        bool? HasEpisodeArtwork { get; set; }
        bool? HasImageArtwork { get; set; }
        bool? HasMovieArtwork { get; set; }
        bool? HasSeasonArtwork { get; set; }
        bool? HasSeriesArtwork { get; set; }
        string Holiday { get; set; }
        KeyWords KeyWords { get; set; }
        string Md5 { get; set; }
        List<ProgramMetadata> Metadata { get; set; }
        Movie Movie { get; set; }
        string OfficialURL { get; set; }
        string OriginalAirDate { get; set; }
        string ProgramID { get; set; }
        string ResourceID { get; set; }
        string ShowType { get; set; }
        List<Title> Titles { get; set; }
    }
}