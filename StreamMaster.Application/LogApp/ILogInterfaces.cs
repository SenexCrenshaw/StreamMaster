using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;

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

public interface ILogTasks
{
}

public interface ILogScoped
{ }
