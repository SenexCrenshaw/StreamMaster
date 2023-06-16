using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;

using StreamMasterInfrastructure.MiddleWare;
using StreamMasterInfrastructure.Persistence;
using StreamMasterInfrastructure.Persistence.Interceptors;
using StreamMasterInfrastructure.Services;
using StreamMasterInfrastructure.Services.Frontend.Mappers;

using System.Reflection;

namespace StreamMasterInfrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
       
        // Dynamically find and register services implementing IMapHttpRequestsToDisk
        var assembly = Assembly.GetExecutingAssembly(); // Or replace with the assembly where your services are defined
        var mapHttpRequestsToDiskImplementations = assembly.GetTypes()
            .Where(type => typeof(IMapHttpRequestsToDisk).IsAssignableFrom(type) && !type.IsInterface);

        foreach (var implementation in mapHttpRequestsToDiskImplementations)
        {
            if (implementation.Name.EndsWith("Base") || implementation.Name.Contains("ogin"))
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

        string DbPath = Path.Join(Constants.DataDirectory, setting.DatabaseName);

        _ = services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        _ = services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={DbPath}", builder => builder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        _ = services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        _ = services.AddScoped<AppDbContextInitialiser>();

        _ = services.AddTransient<IDateTime, DateTimeService>();

        _ = services.AddSingleton<IRingBufferManager, RingBufferManager>();

        _ = services.AddHostedService<TimerService>();

        return services;
    }
}
