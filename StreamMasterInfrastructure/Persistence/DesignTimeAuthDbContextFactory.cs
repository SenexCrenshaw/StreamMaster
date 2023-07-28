using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

using StreamMasterDomain.Common;

namespace StreamMasterInfrastructure.Persistence;

public class DesignTimeAuthDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private readonly string DbPath = Path.Join(BuildInfo.DataFolder, "file.db");

    public AppDbContext CreateDbContext(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        _ = services.AddDbContext<AppDbContext>(options => options.UseSqlite(DbPath));

        AppDbContext? context = services.BuildServiceProvider().GetService<AppDbContext>();
        if (context == null)
        {
            throw new ApplicationException("Couldnt create context");
        }
        FileUtil.CreateDirectory(DbPath);
        return context;
    }
}
