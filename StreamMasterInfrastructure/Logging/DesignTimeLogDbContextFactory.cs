using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

using StreamMasterDomain.Common;

namespace StreamMasterInfrastructure.Logging;

public class DesignTimeLogDbContextFactory : IDesignTimeDbContextFactory<LogDbContext>
{
    private readonly string DbPath = Path.Join(Constants.DataDirectory, "file.db");

    public LogDbContext CreateDbContext(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        _ = services.AddDbContext<LogDbContext>(options => options.UseSqlite(DbPath));

        LogDbContext? context = services.BuildServiceProvider().GetService<LogDbContext>();
        if (context == null)
        {
            throw new ApplicationException("Couldnt create context");
        }
        FileUtil.CreateDirectory(DbPath);
        return context;
    }
}
