
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using StreamMaster.Application.LogApp;
using StreamMaster.Infrastructure.EF.PGSQL.Logging;

namespace StreamMaster.Infrastructure.EF.PGSQL;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureEFPGSQLServices(this IServiceCollection services)
    {

        NpgsqlDataSourceBuilder dataSourceBuilder = new(PGSQLRepositoryContext.DbConnectionString);
        dataSourceBuilder.UseNodaTime();
        NpgsqlDataSource dataSource = dataSourceBuilder.Build();

        _ = services.AddDbContextFactory<PGSQLRepositoryContext>(options =>
            {
                //options.EnableSensitiveDataLogging();
                options.UseNpgsql(dataSource, pgsqlOptions =>
                {
                    _ = pgsqlOptions.MigrationsAssembly(typeof(PGSQLRepositoryContext).Assembly.FullName);
                    pgsqlOptions.UseNodaTime();

                });
            }
        );

        _ = services.AddDbContextFactory<LogDbContext>(options =>
        {
            options.UseNpgsql(
               LogDbContext.DbConnectionString,
                pgsqlOptions => pgsqlOptions.MigrationsAssembly(typeof(LogDbContext).Assembly.FullName)
            );
        }
        );

        _ = services.AddScoped<ILogDB>(provider => provider.GetRequiredService<LogDbContext>());


        return services;
    }
}
