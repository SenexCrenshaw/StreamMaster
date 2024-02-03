
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.LogApp;
using StreamMaster.Infrastructure.EF.PGSQL.Logging;
using StreamMaster.Infrastructure.EF.PGSQL;

namespace StreamMaster.Infrastructure.EF.PGSQL;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFServices(this IServiceCollection services)
    {

        string DbPath = Path.Join(BuildInfo.DataFolder, "StreamMasterPsql.db");
        string LogDbPath = Path.Join(BuildInfo.DataFolder, "StreamMasterPsql_Log.db");

        _ = services.AddDbContextFactory<RepositoryContext>(options =>
            options.UseNpgsql(
                $"Host=postgres;Data Source={DbPath};Username=postgres;Password=sm123",
                builder => builder.MigrationsAssembly(typeof(RepositoryContext).Assembly.FullName)
            )
        );

        _ = services.AddDbContextFactory<LogDbContext>(options =>
            options.UseNpgsql(
                $"Host=postgres;Data Source={LogDbPath};Username=postgres;Password=sm123",
                builder => builder.MigrationsAssembly(typeof(RepositoryContext).Assembly.FullName)
            )
        );

        _ = services.AddScoped<LogDbContextInitialiser>();

        _ = services.AddScoped<ILogDB>(provider => provider.GetRequiredService<LogDbContext>());

        _ = services.AddScoped<RepositoryContextInitializer>();

        //_ = services.AddScoped<ISortHelper<M3UFile>, SortHelper<M3UFile>>();
        //_ = services.AddScoped<ISortHelper<VideoStream>, SortHelper<VideoStream>>();
        //_ = services.AddScoped<ISortHelper<ChannelGroup>, SortHelper<ChannelGroup>>();
        //_ = services.AddScoped<ISortHelper<StreamGroup>, SortHelper<StreamGroup>>();
        //_ = services.AddScoped<ISortHelper<EPGFile>, SortHelper<EPGFile>>();
        _ = services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        return services;
    }
}
