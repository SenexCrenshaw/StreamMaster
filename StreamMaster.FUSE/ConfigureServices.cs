using Microsoft.Extensions.DependencyInjection;

namespace StreamMaster.FUSE;

public static class ConfigureServices
{
    public static IServiceCollection AddFUSEServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystemManager, FileSystemManager>();
        services.AddHostedService<FileSystemSocketService>();
        //services.AddHostedService<FileSystemSocketService>(sp =>
        //{
        //    IFileSystemManager fsManager = sp.GetRequiredService<IFileSystemManager>();
        //    ILogger<FileSystemSocketService> logger = sp.GetRequiredService<ILogger<FileSystemSocketService>>();
        //    // Adjust the socket path as needed if you prefer a different location
        //    return new FileSystemSocketService("/tmp/fuse_middleman.sock", fsManager, logger);
        //});

        return services;
    }
}