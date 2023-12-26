namespace StreamMaster.SchedulesDirect.Domain.Models;

public class ImageDownloadServiceStatus
{
    public int Id { get; set; } = 0;
    public int TotalDownloadAttempts { get; set; }
    public int TotalInQueue { get; set; }
    public int TotalSuccessful { get; set; }
    public int TotalAlreadyExists { get; set; }
    public int TotalNoArt { get; set; }
    public int TotalErrors { get; set; }
}
