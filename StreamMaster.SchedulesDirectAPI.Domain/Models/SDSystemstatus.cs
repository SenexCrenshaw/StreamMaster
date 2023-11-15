using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class SDSystemStatus : ISDSystemstatus
{
    public DateTime date { get; set; }
    public string message { get; set; }
    public string status { get; set; }
}
