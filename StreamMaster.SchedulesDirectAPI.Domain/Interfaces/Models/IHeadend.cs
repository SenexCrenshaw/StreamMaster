namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IHeadend
    {
        string headend { get; set; }
        Lineup[] lineups { get; set; }
        string location { get; set; }
        string transport { get; set; }
    }
}