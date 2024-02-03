
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.Infrastructure.EF.SQLite;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFSQLiteServices(this IServiceCollection services)
    {

        string DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster.db");
        string LogDbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster_Log.db");

        _ = services.AddDbContextFactory<SQLiteRepositoryContext>(options => options.UseSqlite($"Data Source={DbPath}", builder => builder.MigrationsAssembly(typeof(SQLiteRepositoryContext).Assembly.FullName)));
        //_ = services.AddDbContext<LogDbContext>(options => options.UseSqlite($"Data Source={LogDbPath}", builder => builder.MigrationsAssembly(typeof(LogDbContext).Assembly.FullName)));
        //_ = services.AddScoped<LogDbContextInitialiser>();

        //_ = services.AddScoped<ILogDB>(provider => provider.GetRequiredService<LogDbContext>());

        _ = services.AddScoped<SQLiteRepositoryContextInitializer>();

        //_ = services.AddScoped<ISortHelper<M3UFile>, SortHelper<M3UFile>>();
        //_ = services.AddScoped<ISortHelper<VideoStream>, SortHelper<VideoStream>>();
        //_ = services.AddScoped<ISortHelper<ChannelGroup>, SortHelper<ChannelGroup>>();
        //_ = services.AddScoped<ISortHelper<StreamGroup>, SortHelper<StreamGroup>>();
        //_ = services.AddScoped<ISortHelper<EPGFile>, SortHelper<EPGFile>>();
        _ = services.AddScoped<ISQLiteRepositoryWrapper, SQLiteRepositoryWrapper>();
        return services;
    }
}
