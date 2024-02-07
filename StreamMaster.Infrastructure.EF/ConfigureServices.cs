using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Infrastructure.EF.PGSQL;

namespace StreamMaster.Infrastructure.EF;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFServices(this IServiceCollection services)
    {
        _ = services.AddScoped<RepositoryContextInitializer>();
        _ = services.AddScoped<LogDbContextInitialiser>();
        _ = services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        _ = services.AddScoped<IRepositoryContext, PGSQLRepositoryContext>();
        return services;
    }
}
