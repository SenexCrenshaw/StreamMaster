using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.SchedulesDirectAPI.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddSchedulesDirectAPIServices(this IServiceCollection services)
    {

        services.AddSingleton<ISchedulesDirect, SchedulesDirect>();
        services.AddSingleton<ISDService, SDService>();

        return services;
    }
}
