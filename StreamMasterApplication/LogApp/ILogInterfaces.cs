using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.LogApp.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.LogApp;

public interface ILogController
{
    Task<ActionResult<IEnumerable<LogEntryDto>>> GetLogRequest(GetLog request);
}

public interface ILogDB
{
    DbSet<LogEntry> LogEntries { get; set; }
}

public interface ILogHub
{
    Task<IEnumerable<LogEntryDto>> GetLogRequest(GetLog request);
}

public interface ILogTasks
{
}

public interface ILogScoped
{ }
