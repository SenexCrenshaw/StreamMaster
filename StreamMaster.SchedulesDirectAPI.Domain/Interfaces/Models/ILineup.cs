namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ILineup
    {
        string Id { get; set; }
        bool IsDeleted { get; set; }
        string LineupString { get; set; }
        string Location { get; set; }
        string Name { get; set; }
        string Transport { get; set; }
        string Uri { get; set; }
    }
}