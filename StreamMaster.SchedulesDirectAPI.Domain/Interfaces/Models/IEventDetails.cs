namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IEventDetails
    {
        string GameDate { get; set; }
        List<Team> Teams { get; set; }
        string Venue100 { get; set; }
    }
}