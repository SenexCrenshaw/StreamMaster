using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Common.Behaviours;

namespace StreamMaster.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ILoggingUtils, LoggingUtils>();
        //_ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        //_ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));


        return services;
    }
}
