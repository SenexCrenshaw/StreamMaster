namespace StreamMaster.SchedulesDirect;

public class Status
{
    public Account account { get; set; }
    public int code { get; set; }
    public DateTime datetime { get; set; }
    public DateTime lastDataUpdate { get; set; }
    public Lineup[] lineups { get; set; }
    public object[] notifications { get; set; }
    public string serverID { get; set; }
    public Systemstatus[] systemStatus { get; set; }
}
