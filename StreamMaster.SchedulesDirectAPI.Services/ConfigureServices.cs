using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.SchedulesDirectAPI.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddSchedulesDirectAPIServices(this IServiceCollection services)
    {

        services.AddSingleton<ISDToken, SDToken>();
        services.AddSingleton<ISchedulesDirectAPI, SchedulesDirectAPI>();
        services.AddSingleton<ISchedulesDirect, SchedulesDirect>();        
        
        return services;
    }
}
