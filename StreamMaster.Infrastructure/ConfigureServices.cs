using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.LogApp;
using StreamMaster.Domain.Common;
using StreamMaster.Domain.Services;
using StreamMaster.Infrastructure.Logger;
using StreamMaster.Infrastructure.Logging;
using StreamMaster.Infrastructure.Middleware;
using StreamMaster.Infrastructure.Services;
using StreamMaster.Infrastructure.Services.Downloads;
using StreamMaster.Infrastructure.Services.Frontend.Mappers;
using StreamMaster.Infrastructure.Services.Settings;
using StreamMaster.Infrastructure.VideoStreamManager.Channels;
using StreamMaster.Infrastructure.VideoStreamManager.Clients;
using StreamMaster.Infrastructure.VideoStreamManager.Factories;
using StreamMaster.Infrastructure.VideoStreamManager.Statistics;
using StreamMaster.Infrastructure.VideoStreamManager.Streams;

using System.Reflection;

namespace StreamMaster.Infrastructure;

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
        _ = services.AddSingleton<IJobStatusService, JobStatusService>();

        _ = services.AddSingleton<IFileLoggingServiceFactory, FileLoggingServiceFactory>();

        // If needed, you can also pre-register specific instances
        _ = services.AddSingleton(provider =>
        {
            IFileLoggingServiceFactory factory = provider.GetRequiredService<IFileLoggingServiceFactory>();
            return factory.Create("FileLogger");
        });

        _ = services.AddSingleton(provider =>
        {
            IFileLoggingServiceFactory factory = provider.GetRequiredService<IFileLoggingServiceFactory>();
            return factory.Create("FileLoggerDebug");
        });

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
            Assembly.Load("StreamMaster.Domain"),
            Assembly.Load("StreamMaster.Application"),
            Assembly.Load("StreamMaster.Infrastructure")
        );

        _ = services.AddMediatR(cfg =>
        {
            _ = cfg.RegisterServicesFromAssemblies(
                Assembly.Load("StreamMaster.Domain"),
                Assembly.Load("StreamMaster.Application"),
                Assembly.Load("StreamMaster.Infrastructure")
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
