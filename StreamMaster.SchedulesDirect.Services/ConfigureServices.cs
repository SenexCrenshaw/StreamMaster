using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Services;
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
        _ = services.AddSingleton<IXMLTVBuilder, XMLTVBuilder>();


        return services;
    }
}
