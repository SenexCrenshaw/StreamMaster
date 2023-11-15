
namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class SDStatus : ISDStatus
{
    public Account account { get; set; }
    public int code { get; set; }
    public DateTime datetime { get; set; }
    public DateTime lastDataUpdate { get; set; }
    public List<Lineup> lineups { get; set; } = new();
    public object[] notifications { get; set; }
    public string serverID { get; set; }
    public List<SDSystemStatus> systemStatus { get; set; } = new();
}
