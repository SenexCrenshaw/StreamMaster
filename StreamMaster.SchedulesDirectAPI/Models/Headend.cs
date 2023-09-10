namespace StreamMaster.SchedulesDirectAPI.Models;

public class Headend
{
    public string headend { get; set; }
    public Lineup[] lineups { get; set; }
    public string location { get; set; }
    public string transport { get; set; }
}
