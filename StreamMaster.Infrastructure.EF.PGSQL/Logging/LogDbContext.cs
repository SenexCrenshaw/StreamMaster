using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.LogApp;

namespace StreamMaster.Infrastructure.EF.PGSQL.Logging;

public class LogDbContext : DbContext, ILogDB
{

    public static string DbConnectionString
    {
        get
        {
            Setting? setting = FileUtil.GetSetting();
            return $"Host={setting.DB.DBHost};Database={setting.DB.DBName + "_Log"};Username={setting.DB.DBUser};Password={setting.DB.DBPassword}";
        }
    }

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
        //modelBuilder.HasCollation("sm_collation", locale: "C.utf8", provider: "icu", deterministic: false);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //configurationBuilder.Properties<string>().UseCollation("sm_collation");
    }
}
