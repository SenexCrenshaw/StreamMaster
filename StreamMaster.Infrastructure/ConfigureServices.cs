using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
using StreamMaster.SchedulesDirect.Helpers;


using System.Reflection;

namespace StreamMaster.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        _ = services.AddMemoryCache();

        _ = services.AddSingleton<ISettingsService, SettingsService>();
        _ = services.AddSingleton<IIconService, IconService>();
        _ = services.AddSingleton<IImageDownloadQueue, ImageDownloadQueue>();
        _ = services.AddSingleton<ICacheableSpecification, CacheableSpecification>();
        _ = services.AddSingleton<IJobStatusService, JobStatusService>();
        _ = services.AddSingleton<IEPGHelper, EPGHelper>();

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
            Assembly.Load("StreamMaster.Infrastructure"),
            Assembly.Load("StreamMaster.Streams")
        );

        _ = services.AddMediatR(cfg =>
        {
            _ = cfg.RegisterServicesFromAssemblies(
                Assembly.Load("StreamMaster.Domain"),
                Assembly.Load("StreamMaster.Application"),
                Assembly.Load("StreamMaster.Infrastructure"),
                Assembly.Load("StreamMaster.Streams")
            );
        });

        string DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster.db");
        string LogDbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster_Log.db");

        _ = services.AddDbContext<LogDbContext>(options => options.UseSqlite($"Data Source={LogDbPath}", builder => builder.MigrationsAssembly(typeof(LogDbContext).Assembly.FullName)));
        _ = services.AddScoped<LogDbContextInitialiser>();

        _ = services.AddScoped<ILogDB>(provider => provider.GetRequiredService<LogDbContext>());


        _ = services.AddSingleton<IBroadcastService, BroadcastService>();

        _ = services.AddHostedService<TimerService>();

        _ = services.AddSingleton<IImageDownloadService, ImageDownloadService>();
        return services;
    }
}
