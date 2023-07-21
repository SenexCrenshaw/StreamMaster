using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Common;

namespace StreamMasterInfrastructure.Logging;

public class LogDbContext : DbContext
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
        var DbPath = Path.Join(Constants.DataDirectory, "StreamMaster_Log.db");
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //_ = builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
