using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.LogApp;

using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using StreamMasterInfrastructure.Logging;
using StreamMasterInfrastructure.Middleware;
using StreamMasterInfrastructure.Services;
using StreamMasterInfrastructure.Services.Downloads;
using StreamMasterInfrastructure.Services.Frontend.Mappers;
using StreamMasterInfrastructure.Services.Settings;
using StreamMasterInfrastructure.VideoStreamManager.Channels;
using StreamMasterInfrastructure.VideoStreamManager.Clients;
using StreamMasterInfrastructure.VideoStreamManager.Factories;
using StreamMasterInfrastructure.VideoStreamManager.Statistics;
using StreamMasterInfrastructure.VideoStreamManager.Streams;

using System.Reflection;

namespace StreamMasterInfrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        _ = services.AddMemoryCache();
        _ = services.AddSingleton<ISettingsService, SettingsService>();
        _ = services.AddSingleton<IStreamSwitcher, StreamSwitcher>();
        _ = services.AddSingleton<IChannelService, ChannelService>();
        _ = services.AddSingleton<IProxyFactory, ProxyFactory>();
        _ = services.AddSingleton<IClientStreamerManager, ClientStreamerManager>();
        _ = services.AddSingleton<IStreamHandlerFactory, StreamHandlerFactory>();
        _ = services.AddSingleton<IStreamStatisticService, StreamStatisticService>();
        _ = services.AddSingleton<IImageDownloadQueue, ImageDownloadQueue>();
        _ = services.AddSingleton<ICircularRingBufferFactory, CircularRingBufferFactory>();
        _ = services.AddSingleton<IStatisticsManager, StatisticsManager>();
        _ = services.AddSingleton<IInputStatisticsManager, InputStatisticsManager>();
        _ = services.AddSingleton<IStreamManager, StreamManager>();
        _ = services.AddSingleton<ICacheableSpecification, CacheableSpecification>();

        // Dynamically find and register services implementing IMapHttpRequestsToDisk
        Assembly assembly = Assembly.GetExecutingAssembly();
        IEnumerable<Type> mapHttpRequestsToDiskImplementations = assembly.GetTypes()
            .Where(type => typeof(IMapHttpRequestsToDisk).IsAssignableFrom(type) && !type.IsInterface);

        foreach (Type? implementation in mapHttpRequestsToDiskImplementations)
        {
            if (implementation.Name.EndsWith("Base"))
            {
                continue;
            }
            _ = services.AddSingleton(typeof(IMapHttpRequestsToDisk), implementation);
        }

        _ = services.AddAutoMapper(
            Assembly.Load("StreamMasterDomain"),
            Assembly.Load("StreamMasterApplication"),
            Assembly.Load("StreamMasterInfrastructure")
        );

        _ = services.AddMediatR(cfg =>
        {
            _ = cfg.RegisterServicesFromAssemblies(
                Assembly.Load("StreamMasterDomain"),
                Assembly.Load("StreamMasterApplication"),
                Assembly.Load("StreamMasterInfrastructure")
            );
        });

        string DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster.db");
        string LogDbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster_Log.db");

        _ = services.AddDbContext<LogDbContext>(options => options.UseSqlite($"Data Source={LogDbPath}", builder => builder.MigrationsAssembly(typeof(LogDbContext).Assembly.FullName)));
        _ = services.AddScoped<LogDbContextInitialiser>();

        _ = services.AddScoped<ILogDB>(provider => provider.GetRequiredService<LogDbContext>());

        _ = services.AddSingleton<IChannelManager, ChannelManager>();
        _ = services.AddSingleton<IBroadcastService, BroadcastService>();

        _ = services.AddHostedService<TimerService>();
        //_ = services.AddHostedService<ImageDownloadService>();
        _ = services.AddSingleton<IImageDownloadService, ImageDownloadService>();
        return services;
    }
}
