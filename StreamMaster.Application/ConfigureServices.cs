using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.SMChannels;
using StreamMaster.Application.SMStreams;

namespace StreamMaster.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ILoggingUtils, LoggingUtils>();
        //_ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        //_ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        //_ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        services.AddScoped<ISMStreamsService, SMStreamsService>();
        services.AddScoped<ISMChannelsService, SMChannelsService>();
        return services;
    }
}
