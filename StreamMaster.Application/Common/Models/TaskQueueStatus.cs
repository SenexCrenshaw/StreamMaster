namespace StreamMaster.Application.Common.Models;

public class TaskQueueStatusDto
{
    public string Command { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public bool IsRunning { get; set; }
    public DateTime QueueTS { get; set; } = DateTime.Now;
    public DateTime StartTS { get; set; } = DateTime.MinValue;
    public DateTime StopTS { get; set; } = DateTime.MinValue;
}