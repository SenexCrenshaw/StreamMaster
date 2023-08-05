using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Common;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructure.EF
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options)
          : base(options)
        {
            DbPath = Path.Join(BuildInfo.DataFolder, "StreamMaster2.db");
        }
        public string DbPath { get; }
        public DbSet<M3UFile> M3UFiles { get; set; }
        public DbSet<VideoStreamLink> VideoStreamLinks { get; set; }

        public DbSet<VideoStream> VideoStreams { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.StreamGroupVideoStreamConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.VideoStreamLinkConfiguration());
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
