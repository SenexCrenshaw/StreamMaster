using Microsoft.Extensions.DependencyInjection;

using StreamMaster.SchedulesDirectAPI.Converters;
using StreamMaster.SchedulesDirectAPI.Data;

namespace StreamMaster.SchedulesDirectAPI.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddSchedulesDirectAPIServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<IXmltv2Mxf, Xmltv2Mxf>();
        _ = services.AddSingleton<ISchedulesDirectAPI, SchedulesDirectAPI>();
        _ = services.AddSingleton<IEPGCache, EPGCache>();
        _ = services.AddSingleton<ISchedulesDirectData, SchedulesDirectData>();
        _ = services.AddSingleton<ISchedulesDirect, SchedulesDirect>();

        return services;
    }
}
