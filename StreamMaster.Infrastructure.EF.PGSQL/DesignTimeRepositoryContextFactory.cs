using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Infrastructure.EF.PGSQL;

public class DesignTimeRepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{

    public RepositoryContext CreateDbContext(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        _ = services.AddDbContext<RepositoryContext>(options => options.UseNpgsql(RepositoryContext.DbConnectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        RepositoryContext? context = services.BuildServiceProvider().GetService<RepositoryContext>();
        return context == null ? throw new ApplicationException("Couldnt create context") : context;
    }
}
