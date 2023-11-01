using StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class Account : IAccount
{
    public DateTime expires { get; set; }
    public int maxLineups { get; set; }
    public object[] messages { get; set; }
}
