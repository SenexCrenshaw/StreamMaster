using Microsoft.Extensions.DependencyInjection;

using StreamMaster.SchedulesDirect.Converters;
using StreamMaster.SchedulesDirect.Data;
using StreamMaster.SchedulesDirect.Images;
using StreamMaster.SchedulesDirect.Services.Extensions;

namespace StreamMaster.SchedulesDirect.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddSchedulesDirectAPIServices(this IServiceCollection services)
    {
        services.AddEPGCache();

        _ = services.AddSingleton<IXmltv2Mxf, XmlTvToXMLTV>();
        _ = services.AddSingleton<ISchedulesDirectAPIService, SchedulesDirectAPIService>();
        _ = services.AddSingleton<ISchedulesDirectDataService, SchedulesDirectDataService>();
        _ = services.AddSingleton<ISchedulesDirect, SchedulesDirect>();
        _ = services.AddSingleton<IXMLTVBuilder, XMLTVBuilder>();
        _ = services.AddSingleton<IDescriptions, Descriptions>();
        _ = services.AddSingleton<ILineups, Lineups>();
        _ = services.AddSingleton<ISchedules, Schedules>();
        _ = services.AddSingleton<IPrograms, Programs>();
        _ = services.AddSingleton<IKeywords, Keywords>();

        _ = services.AddSingleton<IMovieImages, MovieImages>();
        _ = services.AddSingleton<ISeriesImages, SeriesImages>();
        _ = services.AddSingleton<ISeasonImages, SeasonImages>();
        _ = services.AddSingleton<ISportsImages, SportsImages>();
        return services;
    }
}
