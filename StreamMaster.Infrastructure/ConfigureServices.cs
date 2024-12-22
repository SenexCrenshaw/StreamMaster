using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.Logger;
using StreamMaster.Infrastructure.Middleware;
using StreamMaster.Infrastructure.Services;
using StreamMaster.Infrastructure.Services.Downloads;
using StreamMaster.Infrastructure.Services.Frontend.Mappers;
using StreamMaster.SchedulesDirect.Domain.Interfaces;

namespace StreamMaster.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        _ = services.AddMemoryCache();

        _ = services.AddSingleton<ILogoService, LogoService>();
        _ = services.AddSingleton<IImageDownloadQueue, ImageDownloadQueue>();
        _ = services.AddSingleton<ICacheableSpecification, CacheableSpecification>();
        _ = services.AddSingleton<IJobStatusService, JobStatusService>();
        _ = services.AddSingleton<IEPGHelper, EPGHelper>();
        _ = services.AddSingleton<IFileLoggingServiceFactory, FileLoggingServiceFactory>();
        _ = services.AddSingleton<IMessageService, MessageService>();
        _ = services.AddSingleton<IDataRefreshService, DataRefreshService>();
        _ = services.AddSingleton<IFileUtilService, FileUtilService>();

        _ = services.AddAutoMapper(
            Assembly.Load("StreamMaster.Domain"),
            Assembly.Load("StreamMaster.Application"),
            Assembly.Load("StreamMaster.Infrastructure"),
            Assembly.Load("StreamMaster.Streams"),
             Assembly.Load("StreamMaster.Streams.Domain")
        );

        _ = services.AddMediatR(cfg =>
        {
            _ = cfg.RegisterServicesFromAssemblies(
                Assembly.Load("StreamMaster.Domain"),
                Assembly.Load("StreamMaster.Application"),
                Assembly.Load("StreamMaster.Infrastructure"),
                Assembly.Load("StreamMaster.Streams"),
                 Assembly.Load("StreamMaster.Streams.Domain")
            );
        });
        return services;
    }

    public static IServiceCollection AddInfrastructureServicesEx(this IServiceCollection services)
    {
        _ = services.AddSingleton<IBroadcastService, BroadcastService>();

        _ = services.AddHostedService<TimerService>();

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

        services.AddSingleton<IImageDownloadService, ImageDownloadService>();
        services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<IImageDownloadService>());

        return services;
    }
}