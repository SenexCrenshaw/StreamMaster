using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using StreamMaster.SchedulesDirect.Domain.Models;

using System.Text.RegularExpressions;

namespace StreamMaster.Infrastructure.EF
{
    public class RepositoryContext(DbContextOptions<RepositoryContext> options) : DbContext(options), IDataProtectionKeyContext
    {
        public async Task VacuumDatabaseAsync()
        {
            await Database.ExecuteSqlRawAsync("VACUUM;");
        }

        public async Task MigrateData(List<MxfService> allServices)
        {
            var currentMigration = Database.GetAppliedMigrations().LastOrDefault();
            if (currentMigration == null)
            {
                return;
            }

            if (currentMigration.Equals("20231229192654_SystemKeyValues") && !SystemKeyValues.Any(a => a.Key == "MigratedDB" && a.Value == "20231229192654_SystemKeyValues"))
            {
                var videoStreams = VideoStreams.Where(a => a.User_Tvg_ID != null && a.User_Tvg_ID != "" && !Regex.IsMatch(a.User_Tvg_ID, @"^\d+-")).ToList();

                if (videoStreams.Count == 0)
                {
                    SystemKeyValues.Add(new SystemKeyValue { Key = "MigratedDB", Value = "20231229192654_SystemKeyValues" });
                    await SaveChangesAsync().ConfigureAwait(false);
                    return;
                }

                Console.WriteLine($"Migrating {videoStreams.Count} video streams to new EPGNumber format");
                var updateCount = 0;
                var updated = false;


                foreach (var videoStream in videoStreams)
                {
                    if (!string.IsNullOrEmpty(videoStream.User_Tvg_ID))
                    {
                        var service = allServices.FirstOrDefault(a => a.StationId == videoStream.User_Tvg_ID);
                        if (service != null)
                        {
                            var EPGID = $"{service.EPGNumber}-{videoStream.User_Tvg_ID}";
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
        //public DbSet<ChannelGroupStreamCount> ChannelGroupStreamCounts { get; set; }

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

    }
}