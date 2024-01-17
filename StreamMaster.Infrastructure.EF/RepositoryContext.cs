using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using StreamMaster.SchedulesDirect.Domain.Models;

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace StreamMaster.Infrastructure.EF
{
    public class RepositoryContext(DbContextOptions<RepositoryContext> options) : DbContext(options), IDataProtectionKeyContext
    {
        public async Task VacuumDatabaseAsync()
        {
            await Database.ExecuteSqlRawAsync("VACUUM;");
        }

        public bool IsEntityTracked<TEntity>(TEntity entity) where TEntity : class
        {
            return ChangeTracker.Entries<TEntity>().Any(e => e.Entity == entity);
        }

        public async Task MigrateData(List<MxfService> allServices)
        {
            string? currentMigration = Database.GetAppliedMigrations().LastOrDefault();
            if (currentMigration == null)
            {
                return;
            }
            await Migrate_SystemKeyValues(allServices);

            await VidShortId();
        }


        private async Task VidShortId()
        {
            string? currentMigration = Database.GetAppliedMigrations().LastOrDefault();
            if (currentMigration == null)
            {
                return;
            }
            if (currentMigration.Equals("20240115145416_VidShortId") && !SystemKeyValues.Any(a => a.Key == "MigratedDB" && a.Value == "20240115145416_VidShortId"))
            {
                List<VideoStream> videoStreams = [.. VideoStreams.Where(a => string.IsNullOrEmpty(a.ShortId) || a.ShortId == UniqueHexGenerator.ShortIdEmpty)];

                if (videoStreams.Count == 0)
                {
                    SystemKeyValues.Add(new SystemKeyValue { Key = "MigratedDB", Value = "20240115145416_VidShortId" });
                    await SaveChangesAsync().ConfigureAwait(false);
                    return;
                }

                Console.WriteLine($"Setting {videoStreams.Count} video streams short Id");
                int updateCount = 0;

                ConcurrentDictionary<string, byte> generatedIdsDict = new();

                foreach (VideoStream? videoStream in videoStreams)
                {

                    videoStream.ShortId = UniqueHexGenerator.GenerateUniqueHex(generatedIdsDict);

                    ++updateCount;

                    if (updateCount % 500 == 0)
                    {
                        updateCount = 0;
                        await SaveChangesAsync().ConfigureAwait(false);
                    }

                }

                SystemKeyValues.Add(new SystemKeyValue { Key = "MigratedDB", Value = "20240115145416_VidShortId" });
                await SaveChangesAsync().ConfigureAwait(false);

                Console.WriteLine("Completed setting video streams short Id");

            }

        }

        private async Task Migrate_SystemKeyValues(List<MxfService> allServices)
        {
            string? currentMigration = Database.GetAppliedMigrations().LastOrDefault();
            if (currentMigration == null)
            {
                return;
            }
            if (currentMigration.Equals("20231229192654_SystemKeyValues") && !SystemKeyValues.Any(a => a.Key == "MigratedDB" && a.Value == "20231229192654_SystemKeyValues"))
            {
                List<VideoStream> videoStreams = VideoStreams.Where(a => a.User_Tvg_ID != null && a.User_Tvg_ID != "" && !Regex.IsMatch(a.User_Tvg_ID, @"^\d+-")).ToList();

                if (videoStreams.Count == 0)
                {
                    SystemKeyValues.Add(new SystemKeyValue { Key = "MigratedDB", Value = "20231229192654_SystemKeyValues" });
                    await SaveChangesAsync().ConfigureAwait(false);
                    return;
                }

                Console.WriteLine($"Migrating {videoStreams.Count} video streams to new EPGNumber format");
                int updateCount = 0;
                bool updated = false;


                foreach (VideoStream? videoStream in videoStreams)
                {
                    if (!string.IsNullOrEmpty(videoStream.User_Tvg_ID))
                    {
                        MxfService? service = allServices.FirstOrDefault(a => a.StationId == videoStream.User_Tvg_ID);
                        if (service != null)
                        {
                            string EPGID = $"{service.EPGNumber}-{videoStream.User_Tvg_ID}";
                            videoStream.User_Tvg_ID = EPGID;
                            updated = true;
                        }

                    }

                    if (updated)
                    {
                        ++updateCount;
                        updated = false;
                    }
                    else
                    {
                        continue;
                    }

                    if (updateCount % 500 == 0)
                    {
                        updateCount = 0;
                        await SaveChangesAsync().ConfigureAwait(false);
                    }

                }

                SystemKeyValues.Add(new SystemKeyValue { Key = "MigratedDB", Value = "20231229192654_SystemKeyValues" });
                await SaveChangesAsync().ConfigureAwait(false);

                Console.WriteLine("Completed migrating data to new EPGNumber format");

            }

        }


        public string DbPath { get; } = Path.Join(BuildInfo.DataFolder, "StreamMaster.db");

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

            _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryContext).Assembly);
            modelBuilder.UseCollation("NOCASE_UTF8");

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
            base.OnModelCreating(modelBuilder);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            FileUtil.SetupDirectories();
            _ = options.UseSqlite(
                $"Data Source={DbPath}",
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                );
            SQLitePCL.Batteries.Init();
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


    }
}