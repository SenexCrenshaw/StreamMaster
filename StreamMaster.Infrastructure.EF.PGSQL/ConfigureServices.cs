
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using StreamMaster.Application.LogApp;
using StreamMaster.Infrastructure.EF.PGSQL.Logging;

namespace StreamMaster.Infrastructure.EF.PGSQL;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFServices(this IServiceCollection services)
    {

        NpgsqlDataSourceBuilder dataSourceBuilder = new(RepositoryContext.DbConnectionString);
        dataSourceBuilder.UseNodaTime();
        NpgsqlDataSource dataSource = dataSourceBuilder.Build();

        _ = services.AddDbContextFactory<RepositoryContext>(options =>
            options.UseNpgsql(dataSource, pgsqlOptions =>
            {
                pgsqlOptions.MigrationsAssembly(typeof(RepositoryContext).Assembly.FullName);
                pgsqlOptions.UseNodaTime();
            }
            )//.ReplaceService<IQueryTranslationPostprocessorFactory, MyQueryTranslationPostprocessorFactory>()
        );

        _ = services.AddDbContextFactory<LogDbContext>(options =>
        {
            options.UseNpgsql(
               LogDbContext.DbConnectionString,
                pgsqlOptions => pgsqlOptions.MigrationsAssembly(typeof(LogDbContext).Assembly.FullName)
            );//.ReplaceService<IQueryTranslationPostprocessorFactory, MyQueryTranslationPostprocessorFactory>();
        }
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
