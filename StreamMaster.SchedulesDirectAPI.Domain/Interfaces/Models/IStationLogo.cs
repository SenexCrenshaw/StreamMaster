namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IStationLogo : ILogo
    {
        string Category { get; set; }
        string Source { get; set; }
    }
}