namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ISDSystemstatus
    {
        DateTime date { get; set; }
        string message { get; set; }
        string status { get; set; }
    }
}