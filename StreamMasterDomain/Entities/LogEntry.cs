using Microsoft.Extensions.Logging;

namespace StreamMasterDomain.Entities;

public class LogEntry
{
    public int Id { get; set; }
    public LogLevel LogLevel { get; set; }
    public string LogLevelName => LogLevel.ToString();
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }
}
