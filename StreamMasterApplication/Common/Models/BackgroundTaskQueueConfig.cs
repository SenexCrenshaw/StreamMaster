namespace StreamMasterApplication.Common.Models;

public class BackgroundTaskQueueConfig
{
    public CancellationToken CancellationToken { get; set; }
    public SMQueCommand Command { get; set; }
    public object? Entity { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
}
