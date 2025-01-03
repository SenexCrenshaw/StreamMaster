using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.Base;

namespace StreamMaster.Infrastructure.EF.PGSQL
{
    public partial class PGSQLRepositoryContext(DbContextOptions<PGSQLRepositoryContext> options, ILogger<PGSQLRepositoryContext> logger) : BaseRepositoryContext(options)
    {
        public static string DbConnectionString => $"Host={BuildInfo.DBHost};Database={BuildInfo.DBName};Username={BuildInfo.DBUser};Password={BuildInfo.DBPassword}";

        public new bool IsEntityTracked<TEntity>(TEntity entity) where TEntity : class
        {
            return ChangeTracker.Entries<TEntity>().Any(e => e.Entity == entity);
        }

        public new void MigrateData()
        {
            ApplyCustomSqlScripts();

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
            return;
        }

        /// <summary>
        /// Executes all SQL scripts from the "Scripts" folder in alphabetical order.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown when the "Scripts" directory does not exist or no .sql files are found.</exception>
        public void ApplyCustomSqlScripts()
        {
            string scriptsDirectory = Path.Combine(AppContext.BaseDirectory, "Scripts");

            if (!Directory.Exists(scriptsDirectory))
            {
                throw new FileNotFoundException($"SQL scripts directory not found: {scriptsDirectory}");
            }

            List<string> sqlFiles = [.. Directory.GetFiles(scriptsDirectory, "*.sql").OrderBy(Path.GetFileName)];

            if (sqlFiles.Count == 0)
            {
                throw new FileNotFoundException($"No SQL script files found in directory: {scriptsDirectory}");
            }

            foreach (string filePath in sqlFiles)
            {
                string scriptContent = File.ReadAllText(filePath);

                // Log or indicate the file being executed
                //Console.WriteLine($"Executing script: {Path.GetFileName(filePath)}");
                logger.LogInformation($"Executing script: {Path.GetFileName(filePath)}");

                try
                {
                    // Execute the SQL script
                    Database.ExecuteSqlRaw(scriptContent);
                }
                catch (Exception ex)
                {
                    // Log error and rethrow or handle it based on your requirements
                    Console.Error.WriteLine($"Error executing script {Path.GetFileName(filePath)}: {ex.Message}");
                    throw;
                }
            }
        }

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
            Setting? setting = SettingsHelper.GetSetting<Setting>(BuildInfo.SettingsFile);
            if (setting?.EnableDBDebug == true)
            {
                options.EnableSensitiveDataLogging(true);
            }
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
        }
    }
}