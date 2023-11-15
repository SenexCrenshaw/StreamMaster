namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IMap
    {
        int AtscMajor { get; set; }
        int AtscMinor { get; set; }
        string StationId { get; set; }
        int UhfVhf { get; set; }
    }
}