namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IAccount
    {
        DateTime expires { get; set; }
        int maxLineups { get; set; }
        object[] messages { get; set; }
    }
}