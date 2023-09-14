﻿using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.LogApp;

using StreamMasterDomain.Common;
using StreamMasterDomain.Models;

namespace StreamMasterInfrastructure.Logging;

public class LogDbContext : DbContext, ILogDB
{
    private readonly string DbPath = "";

    public LogDbContext(DbContextOptions<LogDbContext> options)
      : base(options)
    {
        DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster_Log.db");
    }

    public LogDbContext()
    {
        DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster_Log.db");
    }

    public DbSet<LogEntry> LogEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        optionsBuilder.UseSqlite(
              $"Data Source={DbPath}",
              o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
              );
        SQLitePCL.Batteries.Init();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
