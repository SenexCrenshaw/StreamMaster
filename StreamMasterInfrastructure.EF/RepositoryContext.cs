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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
