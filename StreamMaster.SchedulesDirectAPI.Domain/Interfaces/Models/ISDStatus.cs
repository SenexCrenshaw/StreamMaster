

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

public interface ISDStatus
{
    Account account { get; set; }
    int code { get; set; }
    DateTime datetime { get; set; }
    DateTime lastDataUpdate { get; set; }
    List<Lineup> lineups { get; set; }
    object[] notifications { get; set; }
    string serverID { get; set; }
    List<SDSystemStatus> systemStatus { get; set; }
}