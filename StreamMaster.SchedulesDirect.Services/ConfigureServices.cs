using Microsoft.Extensions.DependencyInjection;

using StreamMaster.SchedulesDirect.Cache;
using StreamMaster.SchedulesDirect.Converters;
using StreamMaster.SchedulesDirect.Data;

namespace StreamMaster.SchedulesDirect.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddSchedulesDirectAPIServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<IXmltv2Mxf, XmlTv2Mxf>();
        _ = services.AddSingleton<ISchedulesDirectAPIService, SchedulesDirectAPIService>();
        _ = services.AddSingleton<IEPGCache, EPGCache>();
        _ = services.AddSingleton<ISchedulesDirectDataService, SchedulesDirectDataService>();
        _ = services.AddSingleton<ISchedulesDirect, SchedulesDirect>();

        return services;
    }
}
