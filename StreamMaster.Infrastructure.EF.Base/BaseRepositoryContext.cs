using EFCore.BulkExtensions;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace StreamMaster.Infrastructure.EF.Base;

public class BaseRepositoryContext(DbContextOptions options) : DbContext(options), IDataProtectionKeyContext, IRepositoryContext
{
    public DbSet<SystemKeyValue> SystemKeyValues { get; set; }
    public DbSet<EPGFile> EPGFiles { get; set; }
    public DbSet<M3UFile> M3UFiles { get; set; }
    public DbSet<ChannelGroup> ChannelGroups { get; set; }
    public DbSet<SMStream> SMStreams { get; set; }
    public DbSet<SMChannel> SMChannels { get; set; }
    public DbSet<SMChannelStreamLink> SMChannelStreamLinks { get; set; }
    public DbSet<StreamGroupSMChannelLink> StreamGroupSMChannels { get; set; }
    public DbSet<StreamGroupChannelGroup> StreamGroupChannelGroups { get; set; }
    public DbSet<StreamGroup> StreamGroups { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public DbSet<StreamGroupProfile> StreamGroupProfiles { get; set; }
    public DbSet<StreamGroupSMChannelLink> StreamGroupSMChannelLinks { get; set; }

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

    public bool IsEntityTracked<TEntity>(TEntity entity) where TEntity : class
    {
        return ChangeTracker.Entries<TEntity>().Any(e => e.Entity == entity);
    }

    public Task MigrateData()
    {
        return Task.CompletedTask;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityAlwaysColumns();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseRepositoryContext).Assembly);

        //_ = modelBuilder.Entity<VideoStream>()
        // .HasIndex(e => e.User_Tvg_group)
        // .HasDatabaseName("idx_User_Tvg_group");

        //// Composite index on User_Tvg_group and IsHidden
        //_ = modelBuilder.Entity<VideoStream>()
        //    .HasIndex(e => new { e.User_Tvg_group, e.IsHidden })
        //    .HasDatabaseName("idx_User_Tvg_group_IsHidden");

        _ = modelBuilder.Entity<ChannelGroup>()
           .HasIndex(e => e.Name)
           .HasDatabaseName("idx_Name");

        _ = modelBuilder.Entity<SMChannel>()
          .HasIndex(e => e.Name)
          .HasDatabaseName("idx_SMChannelName");

        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => e.Name)
            .HasDatabaseName("idx_SMStreamName");

        // Composite index on User_Tvg_group and IsHidden
        _ = modelBuilder.Entity<ChannelGroup>()
            .HasIndex(e => new { e.Name, e.IsHidden })
            .HasDatabaseName("idx_Name_IsHidden");

        //_ = modelBuilder.Entity<VideoStream>()
        //  .HasIndex(e => e.User_Tvg_name)
        //  .HasDatabaseName("IX_VideoStream_User_Tvg_name");

        //modelBuilder.Entity<VideoStream>()
        //    .HasIndex(p => p.User_Tvg_chno)
        //    .HasDatabaseName("IX_VideoStream_User_Tvg_chno");


        //modelBuilder.Entity<VideoStream>()
        //    .HasIndex(p => p.ShortSMChannelId)
        //    .HasDatabaseName("IX_VideoStream_SMChannelId");

        //modelBuilder.OnHangfireModelCreating();

        modelBuilder.ApplyUtcDateTimeConverter();

        base.OnModelCreating(modelBuilder);

    }

}
