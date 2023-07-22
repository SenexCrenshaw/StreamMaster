namespace StreamMaster.SchedulesDirectAPI;

public class SDStatus
{
    public Account account { get; set; }
    public int code { get; set; }
    public DateTime datetime { get; set; }
    public DateTime lastDataUpdate { get; set; }
    public Lineup[] lineups { get; set; }
    public object[] notifications { get; set; }
    public string serverID { get; set; }
    public SDSystemstatus[] systemStatus { get; set; }
}
