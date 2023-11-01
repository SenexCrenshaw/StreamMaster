namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Headend : IHeadend
{
    public string headend { get; set; }
    public Lineup[] lineups { get; set; }
    public string location { get; set; }
    public string transport { get; set; }
}
