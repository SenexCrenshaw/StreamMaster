
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Infrastructure.EF.SQLite;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFSQLiteServices(this IServiceCollection services)
    {

        string DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster.db");
        string LogDbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster_Log.db");

        _ = services.AddDbContextFactory<SQLiteRepositoryContext>(options => options.UseSqlite($"Data Source={DbPath}", builder => builder.MigrationsAssembly(typeof(SQLiteRepositoryContext).Assembly.FullName)));

        return services;
    }
}
