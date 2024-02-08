using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Infrastructure.EF.PGSQL.Logging;

public class DesignTimeLogDbContextFactory : IDesignTimeDbContextFactory<LogDbContext>
{
    public LogDbContext CreateDbContext(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        _ = services.AddDbContext<LogDbContext>(options => options.UseNpgsql(LogDbContext.DbConnectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        LogDbContext? context = services.BuildServiceProvider().GetService<LogDbContext>();
        return context ?? throw new ApplicationException("Couldnt create context");
    }
}
