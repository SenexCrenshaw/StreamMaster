using System.ComponentModel.DataAnnotations.Schema;

using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Models;

[RequireAll]
public class LogEntry
{
    public int Id { get; set; }
    public LogLevel LogLevel { get; set; }
    [Column(TypeName = "citext")]
    public string LogLevelName => LogLevel.ToString();
    [Column(TypeName = "citext")]
    public string Message { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
}