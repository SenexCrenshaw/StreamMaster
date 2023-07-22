﻿using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.LogApp;

using StreamMasterDomain.Common;
using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Logging;

public class LogDbContext : DbContext, ILogDB
{
    private string DbPath = "";

    public LogDbContext(DbContextOptions<LogDbContext> options)
      : base(options)
    {
        DbPath = Path.Join(Constants.DataDirectory, "StreamMaster_Log.db");
    }

    public LogDbContext()
    {
        DbPath = Path.Join(Constants.DataDirectory, "StreamMaster_Log.db");
    }

    public DbSet<LogEntry> LogEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
