namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ILineUpPreview
    {
        string Affiliate { get; set; }
        string Callsign { get; set; }
        string Channel { get; set; }
        int Id { get; set; }
        string LineUp { get; set; }
        string Name { get; set; }
    }
}