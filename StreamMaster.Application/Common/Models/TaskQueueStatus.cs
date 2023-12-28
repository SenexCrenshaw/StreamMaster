namespace StreamMaster.Application.Common.Models;

public class TaskQueueStatus
{
    public string Command { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public bool IsRunning { get; set; }
    public DateTime QueueTS { get; set; } = DateTime.Now;
    public DateTime StartTS { get; set; } = DateTime.MinValue;
    public DateTime StopTS { get; set; } = DateTime.MinValue;
    public string ElapsedTS => GetElapsedTimeFormatted();

    public string GetElapsedTimeFormatted()
    {
        TimeSpan elapsedTime = StopTS - StartTS;
        return $"{elapsedTime.Days} {elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
    }
}