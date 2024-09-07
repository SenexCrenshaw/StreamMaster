using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.ChannelGroups;
using StreamMaster.Application.Crypto;
using StreamMaster.Application.M3UFiles;
using StreamMaster.Application.Profiles;
using StreamMaster.Application.SMChannels;
using StreamMaster.Application.SMStreams;
using StreamMaster.Application.StreamGroups;
using StreamMaster.Domain.Cache;
namespace StreamMaster.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ILoggingUtils, LoggingUtils>();
        //_ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        //_ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        //_ = services.AddTransient(typeof(IPipelineBehavior<,
        services.AddTransient<ICryptoService, CryptoService>();
        services.AddTransient<IProfileService, ProfileService>();
        services.AddScoped<IStreamGroupService, StreamGroupService>();
        services.AddScoped<IM3UFileService, M3UFileService>();
        services.AddScoped<IM3UToSMStreamsService, M3UToSMStreamsService>();
        services.AddScoped<IChannelGroupService, ChannelGroupService>();
        services.AddScoped(typeof(CachedConcurrentDictionary<,>));
        services.AddScoped<ISMChannelService, SMChannelService>();
        services.AddScoped<ISMStreamService, SMStreamService>();

        return services;
    }
}
