namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ISchedule
    {
        ScheduleMetadata Metadata { get; set; }
        List<Program> Programs { get; set; }
        string StationID { get; set; }
    }
}