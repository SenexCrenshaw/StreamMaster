using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;

using StreamMasterInfrastructure.MiddleWare;
using StreamMasterInfrastructure.Persistence;
using StreamMasterInfrastructure.Persistence.Interceptors;
using StreamMasterInfrastructure.Services;

using System.Reflection;

namespace StreamMasterInfrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
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
