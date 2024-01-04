using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Services;
using StreamMaster.SchedulesDirect.Converters;
using StreamMaster.SchedulesDirect.Data;
using StreamMaster.SchedulesDirect.Services.Extensions;

namespace StreamMaster.SchedulesDirect.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddSchedulesDirectAPIServices(this IServiceCollection services)
    {
        services.AddEPGCache();

        _ = services.AddSingleton<IXmltv2Mxf, XmlTv2Mxf>();
        _ = services.AddSingleton<ISchedulesDirectAPIService, SchedulesDirectAPIService>();
        _ = services.AddSingleton<ISchedulesDirectDataService, SchedulesDirectDataService>();
        _ = services.AddSingleton<ISchedulesDirect, SchedulesDirect>();
        _ = services.AddSingleton<IXMLTVBuilder, XMLTVBuilder>();
        _ = services.AddSingleton<IDescriptions, Descriptions>();
        _ = services.AddSingleton<ILineups, Lineups>();
        _ = services.AddSingleton<ISchedules, Schedules>();
        _ = services.AddSingleton<IPrograms, Programs>();
        _ = services.AddSingleton<IKeywords, Keywords>();
        _ = services.AddSingleton<ISchedulesDirectImages, SchedulesDirectImages>();
        return services;
    }
}
