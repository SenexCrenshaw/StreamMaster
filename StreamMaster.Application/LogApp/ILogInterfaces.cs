using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.LogApp.Queries;

namespace StreamMaster.Application.LogApp;

public interface ILogController
{
    Task<ActionResult<IEnumerable<LogEntryDto>>> GetLog(GetLogRequest request);
}

public interface ILogDB
{
    DbSet<LogEntry> LogEntries { get; set; }
}

public interface ILogHub
{
    Task<IEnumerable<LogEntryDto>> GetLog(GetLogRequest request);
}
