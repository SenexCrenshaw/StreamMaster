using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Attributes;

using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

[RequireAll]
public class LogEntry
{
    public int Id { get; set; }
    public LogLevel LogLevel { get; set; }
    [Column(TypeName = "citext")]
    public string LogLevelName => LogLevel.ToString();
    [Column(TypeName = "citext")]
    public string Message { get; set; }
    public DateTime TimeStamp { get; set; }
}