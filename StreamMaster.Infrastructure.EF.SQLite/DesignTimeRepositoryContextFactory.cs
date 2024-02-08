using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Common;

namespace StreamMaster.Infrastructure.EF.SQLite;

public class DesignTimeRepositoryContextFactory : IDesignTimeDbContextFactory<SQLiteRepositoryContext>
{
    private readonly string DbPath = Path.Join(BuildInfo.DataFolder, "file.db");

    public SQLiteRepositoryContext CreateDbContext(string[] args)
    {
        IServiceCollection services = new ServiceCollection();

        _ = services.AddDbContext<SQLiteRepositoryContext>(options => options.UseSqlite(DbPath));

        SQLiteRepositoryContext? context = services.BuildServiceProvider().GetService<SQLiteRepositoryContext>();
        if (context == null)
        {
            throw new ApplicationException("Couldnt create context");
        }
        FileUtil.CreateDirectory(DbPath);
        return context;
    }
}
