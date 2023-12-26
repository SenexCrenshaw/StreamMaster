
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Common;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Sorting;
using StreamMaster.Infrastructure.EF.Helpers;

namespace StreamMaster.Infrastructure.EF;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFServices(this IServiceCollection services)
    {

        string DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster.db");

        //_ = services.AddDbContext<RepositoryContext>(options => options.UseSqlite($"Data Source={DbPath}", builder => builder.MigrationsAssembly(typeof(RepositoryContext).Assembly.FullName)));
        _ = services.AddDbContextFactory<RepositoryContext>(options => options.UseSqlite($"Data Source={DbPath}", builder => builder.MigrationsAssembly(typeof(RepositoryContext).Assembly.FullName)));



        //_ = services.AddHangfire((serviceProvider, configuration) =>
        //    configuration.UseEFCoreStorage(
        //           () => serviceProvider.GetRequiredService<IDbContextFactory<RepositoryContext>>().CreateDbContext(),
        //        new EFCoreStorageOptions
        //        {
        //            CountersAggregationInterval = new TimeSpan(0, 5, 0),
        //            DistributedLockTimeout = new TimeSpan(0, 10, 0),
        //            JobExpirationCheckInterval = new TimeSpan(0, 30, 0),
        //            QueuePollInterval = new TimeSpan(0, 0, 15),
        //            Schema = string.Empty,
        //            SlidingInvisibilityTimeout = new TimeSpan(0, 5, 0),
        //        }));

        //_ = services.AddHangfireServer(options =>
        //{
        //    options.WorkerCount = 1;
        //});

        _ = services.AddScoped<RepositoryContextInitializer>();

        _ = services.AddScoped<ISortHelper<M3UFile>, SortHelper<M3UFile>>();
        _ = services.AddScoped<ISortHelper<VideoStream>, SortHelper<VideoStream>>();
        _ = services.AddScoped<ISortHelper<ChannelGroup>, SortHelper<ChannelGroup>>();
        _ = services.AddScoped<ISortHelper<StreamGroup>, SortHelper<StreamGroup>>();
        _ = services.AddScoped<ISortHelper<EPGFile>, SortHelper<EPGFile>>();
        _ = services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        return services;
    }
}
