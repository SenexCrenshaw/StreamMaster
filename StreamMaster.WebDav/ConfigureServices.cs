using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Configuration;
using StreamMaster.WebDav.Providers;

namespace StreamMaster.WebDav;

public static class ConfigureServices
{
    public static IServiceCollection AddWebDavServices(this IServiceCollection services)
    {
        // Register the LockManager
        services.AddSingleton<ILockManager, LockManager>();

        // Register dependencies for CombinedStorageProvider
        services.AddSingleton<VirtualStorageProvider>();
        services.AddSingleton<LocalCacheStorageProvider>(provider =>
        {
            // Initialize LocalCacheStorageProvider with a cache directory
            // string cacheRoot = Path.Combine(BuildInfo.BaseDirectory, "Cache");
            return new LocalCacheStorageProvider(BuildInfo.WebDavFolder);
        });

        // Register the CombinedStorageProvider as the IWebDavStorageProvider implementation
        services.AddSingleton<IWebDavStorageProvider, CombinedStorageProvider>();

        return services;
    }
}