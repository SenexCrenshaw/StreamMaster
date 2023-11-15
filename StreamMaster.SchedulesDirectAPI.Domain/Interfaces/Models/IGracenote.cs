namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IGracenote
    {
        int Episode { get; set; }
        int Season { get; set; }
        int? TotalEpisodes { get; set; }
        int? TotalSeasons { get; set; }
    }
}