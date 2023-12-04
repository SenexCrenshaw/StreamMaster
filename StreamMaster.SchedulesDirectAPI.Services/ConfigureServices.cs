using Microsoft.Extensions.DependencyInjection;

using StreamMaster.SchedulesDirectAPI.Data;
using StreamMaster.SchedulesDirectAPI.Domain.Enums;

namespace StreamMaster.SchedulesDirectAPI.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddSchedulesDirectAPIServices(this IServiceCollection services)
    {

        services.AddSingleton<ISDToken, SDToken>();
        services.AddSingleton<ISchedulesDirectAPI, SchedulesDirectAPI>();
        services.AddSingleton<IEPGCache, EPGCache>();
        services.AddSingleton<ISchedulesDirectData, SchedulesDirectData>();
        services.AddSingleton<ISchedulesDirect, SchedulesDirect>();        
        
        return services;
    }
}
