namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ILineupPreview
    {
        string Affiliate { get; set; }
        string Callsign { get; set; }
        string Channel { get; set; }
        int Id { get; set; }
        string Lineup { get; set; }
        string Name { get; set; }
    }
}