using Microsoft.Extensions.DependencyInjection;
namespace StreamMaster.SchedulesDirect.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddSchedulesDirectAPIServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<ISchedulesDirectAPIService, SchedulesDirectAPIService>();
        _ = services.AddSingleton<ISchedulesDirectRepository, SchedulesDirectRepository>();
        _ = services.AddSingleton<IHttpService, HttpService>();

        return services;
    }
}