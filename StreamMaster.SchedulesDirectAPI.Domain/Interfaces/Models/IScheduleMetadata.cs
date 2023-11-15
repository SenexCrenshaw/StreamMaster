namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IScheduleMetadata
    {
        string Md5 { get; set; }
        DateTime Modified { get; set; }
        string StartDate { get; set; }
    }
}