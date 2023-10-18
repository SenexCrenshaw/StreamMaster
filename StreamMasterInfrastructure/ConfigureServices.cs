using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.LogApp;
using StreamMasterApplication.Services;

using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using StreamMasterInfrastructure.Logging;
using StreamMasterInfrastructure.Middleware;
using StreamMasterInfrastructure.Services;
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
        services.AddMemoryCache();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IStreamFactory, DefaultStreamFactory>();
        services.AddSingleton<IStreamSwitcher, StreamSwitcher>();
        services.AddSingleton<IChannelService, ChannelService>();
        services.AddSingleton<IProxyFactory, ProxyFactory>();
        services.AddSingleton<IClientManager, ClientStreamerManager>();
        services.AddSingleton<IStreamStatisticService, StreamStatisticService>();

        services.AddSingleton<ICircularRingBufferFactory, CircularRingBufferFactory>();
        services.AddSingleton<IStatisticsManager, StatisticsManager>();
        services.AddSingleton<IInputStatisticsManager, InputStatisticsManager>();
        services.AddSingleton<IStreamManager, StreamManager>();
        services.AddSingleton<ISDService, SDService>();
        services.AddSingleton<ICacheableSpecification, CacheableSpecification>();

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
            services.AddSingleton(typeof(IMapHttpRequestsToDisk), implementation);
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

        _ = services.AddTransient<IDateTime, DateTimeService>();

        _ = services.AddSingleton<IChannelManager, ChannelManager>();
        services.AddSingleton<IBroadcastService, BroadcastService>();

        _ = services.AddHostedService<TimerService>();

        return services;
    }
}
