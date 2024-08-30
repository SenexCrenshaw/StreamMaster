
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

namespace StreamMaster.Infrastructure.EF.PGSQL;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFPGSQLServices(this IServiceCollection services)
    {
        NpgsqlDataSourceBuilder dataSourceBuilder = new(PGSQLRepositoryContext.DbConnectionString);
        _ = dataSourceBuilder.UseNodaTime();
        NpgsqlDataSource dataSource = dataSourceBuilder.Build();

        _ = services.AddDbContextFactory<PGSQLRepositoryContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                _ = options.UseNpgsql(dataSource, pgsqlOptions =>
                {
                    _ = pgsqlOptions.MigrationsAssembly(typeof(PGSQLRepositoryContext).Assembly.FullName);
                    _ = pgsqlOptions.UseNodaTime();
                });
            }
        );

        return services;
    }
}
