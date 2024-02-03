using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.LogApp;
using StreamMaster.Domain.Common;

namespace StreamMaster.Infrastructure.EF.SQLite.Logging;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("NOCASE");
        base.OnModelCreating(modelBuilder);
    }
}
