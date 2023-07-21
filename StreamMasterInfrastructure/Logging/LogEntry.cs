using Microsoft.Extensions.Logging;

namespace StreamMasterInfrastructure.Logging;

public class LogEntry
{
    public int Id { get; set; }
    public LogLevel LogLevel { get; set; }
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }
}
