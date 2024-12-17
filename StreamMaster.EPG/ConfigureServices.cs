using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.EPG;

public static class ConfigureServices
{
    public static IServiceCollection AddEPGServices(this IServiceCollection services)
    {
        services.AddScoped<IEpgMatcher, EpgMatcher>();
        return services;
    }
}