using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Common;
using StreamMasterDomain.Models;

namespace StreamMasterInfrastructureEF
{
    public class RepositoryContext : DbContext, IDataProtectionKeyContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options)
          : base(options)
        {
            DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster.db");
        }

        public string DbPath { get; }
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