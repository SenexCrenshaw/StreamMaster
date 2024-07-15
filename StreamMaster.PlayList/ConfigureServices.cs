using Microsoft.Extensions.DependencyInjection;
namespace StreamMaster.PlayList;

public static class ConfigureServices
{
    public static IServiceCollection AddCustomPlayListServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<ICustomPlayListBuilder, CustomPlayListBuilder>();
        _ = services.AddSingleton<INfoFileReader, NfoFileReader>();


        return services;
    }
}
