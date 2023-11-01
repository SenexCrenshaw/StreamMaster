namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ILineUpResult
    {
        List<Map> Map { get; set; }
        Metadata Metadata { get; set; }
        List<Station> Stations { get; set; }
    }
}