using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Infrastructure.EF.PGSQL;

public class DesignTimeRepositoryContextFactory : IDesignTimeDbContextFactory<PGSQLRepositoryContext>
{

    public PGSQLRepositoryContext CreateDbContext(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        _ = services.AddDbContext<PGSQLRepositoryContext>(options => options.UseNpgsql(PGSQLRepositoryContext.DbConnectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        PGSQLRepositoryContext? context = services.BuildServiceProvider().GetService<PGSQLRepositoryContext>();
        return context == null ? throw new ApplicationException("Couldnt create context") : context;
    }
}
