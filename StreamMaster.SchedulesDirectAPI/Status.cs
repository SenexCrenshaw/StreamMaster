namespace StreamMaster.SchedulesDirectAPI;

public class SDStatus
{
    public Account account { get; set; }
    public int code { get; set; }
    public DateTime datetime { get; set; }
    public DateTime lastDataUpdate { get; set; }
    public List<Lineup> lineups { get; set; } = new();
    public object[] notifications { get; set; }
    public string serverID { get; set; }
    public List<SDSystemstatus> systemStatus { get; set; } = new();
}
