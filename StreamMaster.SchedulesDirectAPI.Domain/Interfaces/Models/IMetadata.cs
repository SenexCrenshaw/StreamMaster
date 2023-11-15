namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IMetadata
    {
        string Lineup { get; set; }
        DateTime Modified { get; set; }
        string Transport { get; set; }
    }
}