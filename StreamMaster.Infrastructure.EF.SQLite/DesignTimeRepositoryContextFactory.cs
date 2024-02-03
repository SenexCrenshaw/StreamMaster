using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Common;

namespace StreamMaster.Infrastructure.EF;

public class DesignTimeRepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    private readonly string DbPath = Path.Join(BuildInfo.DataFolder, "file.db");

    public RepositoryContext CreateDbContext(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        _ = services.AddDbContext<RepositoryContext>(options => options.UseSqlite(DbPath));

        RepositoryContext? context = services.BuildServiceProvider().GetService<RepositoryContext>();
        if (context == null)
        {
            throw new ApplicationException("Couldnt create context");
        }
        FileUtil.CreateDirectory(DbPath);
        return context;
    }
}
