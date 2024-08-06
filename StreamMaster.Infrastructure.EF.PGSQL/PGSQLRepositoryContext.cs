using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.Base;
using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.Infrastructure.EF.PGSQL
{
    public partial class PGSQLRepositoryContext(DbContextOptions<PGSQLRepositoryContext> options, ILoggerFactory loggerFactory, IOptionsMonitor<Setting> _settings) : BaseRepositoryContext(options)
    {

        public static string DbConnectionString => $"Host={BuildInfo.DBHost};Database={BuildInfo.DBName};Username={BuildInfo.DBUser};Password={BuildInfo.DBPassword}";

        public new bool IsEntityTracked<TEntity>(TEntity entity) where TEntity : class
        {
            return ChangeTracker.Entries<TEntity>().Any(e => e.Entity == entity);
        }

        public async Task MigrateData(List<MxfService>? allServices = null)
        {
            string? currentMigration = Database.GetAppliedMigrations().LastOrDefault();
            if (currentMigration == null)
            {
                return;
            }

            //if (currentMigration == "20240229201207_ConvertToSMChannels")
            //{
            //if (!SystemKeyValues.Any(a => a.Key == "MigrateData_20240229201207_ConvertToSMChannels"))
            //{
            //    MigrateData_20240229201207_ConvertToSMChannels();
            //    SystemKeyValues.Add(new SystemKeyValue { Key = "MigrateData_20240229201207_ConvertToSMChannels", Value = "1" });
            //    await SaveChangesAsync().ConfigureAwait(false);
            //}

            //}
        }

        //public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        //{
        //    builder
        //        .AddConsole()
        //        .AddFilter((category, level) =>
        //            category == DbLoggerCategory.Database.Command.Name &&
        //            level == LogLevel.Information)
        //        .AddDebug();
        //});

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            DirectoryHelper.CreateApplicationDirectories();
            options.UseNpgsql(DbConnectionString,
                o =>
                {
                    o.UseNodaTime();
                    o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                }
                );
            if (_settings.CurrentValue.EnableDBDebug)
            {
                options.EnableSensitiveDataLogging(true).UseLoggerFactory(loggerFactory);
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
        }

    }
}