using Microsoft.Extensions.DependencyInjection;

using StreamMaster.SchedulesDirect.Cache;

namespace StreamMaster.SchedulesDirect.Services.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEPGCache(this IServiceCollection services)
    {
        // This registers the open generic type which will be closed by the DI container
        services.AddSingleton(typeof(IEPGCache<>), typeof(EPGCache<>));
        return services;
    }
}
