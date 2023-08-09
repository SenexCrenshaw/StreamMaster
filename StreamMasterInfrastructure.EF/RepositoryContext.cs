using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using StreamMasterDomain.Common;
using StreamMasterDomain.Repository;

using System.Diagnostics;

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

        public DbSet<StreamGroupChannelGroup> StreamGroupChannelGroups { get; set; }
        public DbSet<StreamGroup> StreamGroups { get; set; }
        public DbSet<StreamGroupVideoStream> StreamGroupVideoStreams { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            FileUtil.SetupDirectories();
            _ = options.UseSqlite(
                $"Data Source={DbPath}",
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                );
        }
    }
}