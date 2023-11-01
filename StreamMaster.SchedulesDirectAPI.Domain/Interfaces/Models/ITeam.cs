namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ITeam
    {
        bool? IsHome { get; set; }
        string Name { get; set; }
    }
}