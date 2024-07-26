using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Crypto;
using StreamMaster.Application.Interfaces;
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
        return services;
    }
}
