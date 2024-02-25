using EFCore.BulkExtensions;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;
using StreamMaster.SchedulesDirect.Domain.Models;

using System.Text.RegularExpressions;

namespace StreamMaster.Infrastructure.EF.PGSQL
{
    public partial class PGSQLRepositoryContext(DbContextOptions<PGSQLRepositoryContext> options) : DbContext(options), IDataProtectionKeyContext, IRepositoryContext
    {

        public static string DbConnectionString => $"Host=127.0.0.1;Database={BuildInfo.DBName};Username={BuildInfo.DBUser};Password={BuildInfo.DBPassword}";

        public bool IsEntityTracked<TEntity>(TEntity entity) where TEntity : class
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

            //if (!SystemKeyValues.Any(a => a.Key == "ChangeIDAlways"))
            //{
            //    await FixIDs().ConfigureAwait(false);
            //    SystemKeyValues.Add(new SystemKeyValue { Key = "ChangeIDAlways", Value = "1" });
            //    await SaveChangesAsync().ConfigureAwait(false);
            //}
        }


        public DbSet<SystemKeyValue> SystemKeyValues { get; set; }

        public DbSet<EPGFile> EPGFiles { get; set; }
        public DbSet<M3UFile> M3UFiles { get; set; }
        public DbSet<VideoStreamLink> VideoStreamLinks { get; set; }
        public DbSet<ChannelGroup> ChannelGroups { get; set; }
        public DbSet<VideoStream> VideoStreams { get; set; }

        public DbSet<StreamGroupChannelGroup> StreamGroupChannelGroups { get; set; }
        public DbSet<StreamGroup> StreamGroups { get; set; }
        public DbSet<StreamGroupVideoStream> StreamGroupVideoStreams { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseIdentityAlwaysColumns();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PGSQLRepositoryContext).Assembly);

            _ = modelBuilder.Entity<VideoStream>()
             .HasIndex(e => e.User_Tvg_group)
             .HasDatabaseName("idx_User_Tvg_group");

            // Composite index on User_Tvg_group and IsHidden
            _ = modelBuilder.Entity<VideoStream>()
                .HasIndex(e => new { e.User_Tvg_group, e.IsHidden })
                .HasDatabaseName("idx_User_Tvg_group_IsHidden");

            _ = modelBuilder.Entity<ChannelGroup>()
               .HasIndex(e => e.Name)
               .HasDatabaseName("idx_Name");

            // Composite index on User_Tvg_group and IsHidden
            _ = modelBuilder.Entity<ChannelGroup>()
                .HasIndex(e => new { e.Name, e.IsHidden })
                .HasDatabaseName("idx_Name_IsHidden");

            _ = modelBuilder.Entity<VideoStream>()
              .HasIndex(e => e.User_Tvg_name)
              .HasDatabaseName("IX_VideoStream_User_Tvg_name");

            modelBuilder.Entity<VideoStream>()
                .HasIndex(p => p.User_Tvg_chno)
                .HasDatabaseName("IX_VideoStream_User_Tvg_chno");


            modelBuilder.Entity<VideoStream>()
                .HasIndex(p => p.ShortId)
                .HasDatabaseName("IX_VideoStream_ShortId");

            //modelBuilder.OnHangfireModelCreating();

            modelBuilder.ApplyUtcDateTimeConverter();

            base.OnModelCreating(modelBuilder);

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

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            //configurationBuilder.Properties<string>().UseCollation("sm_collation");
        }

        private bool _disposed = false;

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }


        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {


                if (disposing)
                {
#if DEBUG
                    SqliteConnection.ClearAllPools();
#endif
                    base.Dispose();

                }
            }
            _disposed = true;
        }

        [GeneratedRegex(@"^\d+-")]
        private static partial Regex UserTVGIDRegex();

        public int ExecuteSqlRaw(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlRaw(sql, parameters);
        }

        public Task<int> ExecuteSqlRawAsyncEntities(string sql, CancellationToken cancellationToken = default)
        {
            return Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }

        public void BulkUpdateEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            this.BulkUpdate(entities);
        }

        public void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            this.BulkInsert(entities);
        }

        public Task BulkDeleteAsyncEntities<TEntity>(IQueryable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
        {
            return entities.ExecuteDeleteAsync(cancellationToken);
        }
    }
}