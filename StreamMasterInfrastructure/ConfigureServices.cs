using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.LogApp;

using StreamMasterDomain.Common;

using StreamMasterInfrastructure.Logging;
using StreamMasterInfrastructure.Persistence;
using StreamMasterInfrastructure.Services;
using StreamMasterInfrastructure.Services.Frontend.Mappers;
using StreamMasterInfrastructure.VideoStreamManager;

using System.Reflection;

namespace StreamMasterInfrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Dynamically find and register services implementing IMapHttpRequestsToDisk
        var assembly = Assembly.GetExecutingAssembly();
        var mapHttpRequestsToDiskImplementations = assembly.GetTypes()
            .Where(type => typeof(IMapHttpRequestsToDisk).IsAssignableFrom(type) && !type.IsInterface);

        foreach (var implementation in mapHttpRequestsToDiskImplementations)
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

        Setting setting = FileUtil.GetSetting();

        string DbPath = Path.Join(Constants.DataDirectory, "StreamMaster.db");
        string LogDbPath = Path.Join(Constants.DataDirectory, "StreamMaster_Log.db");

        _ = services.AddDbContext<LogDbContext>(options => options.UseSqlite($"Data Source={LogDbPath}", builder => builder.MigrationsAssembly(typeof(LogDbContext).Assembly.FullName)));
        _ = services.AddScoped<LogDbContextInitialiser>();

        _ = services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={DbPath}", builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));              
        _ = services.AddScoped<AppDbContextInitialiser>();

        _ = services.AddScoped<ILogDB>(provider => provider.GetRequiredService<LogDbContext>());
        _ = services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        
                _ = services.AddTransient<IDateTime, DateTimeService>();

        _ = services.AddSingleton<IChannelManager, ChannelManager>();

        _ = services.AddHostedService<TimerService>();

        return services;
    }
}
