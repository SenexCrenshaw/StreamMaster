using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.LogApp;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.EF.PGSQL.Logging;

public class LogDbContext : DbContext, ILogDB
{

    public static string DbConnectionString => $"Host=127.0.0.1;Database={BuildInfo.DBName + "_Log"};Username={BuildInfo.DBUser};Password={BuildInfo.DBPassword}";

    public LogDbContext(DbContextOptions<LogDbContext> options)
      : base(options)
    {

    }

    public LogDbContext()
    {

    }

    public DbSet<LogEntry> LogEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        optionsBuilder.UseNpgsql(
              DbConnectionString,
              o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
              );

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityAlwaysColumns();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //configurationBuilder.Properties<string>().UseCollation("sm_collation");
    }
}
