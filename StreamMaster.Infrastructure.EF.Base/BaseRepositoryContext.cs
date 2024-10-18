using EFCore.BulkExtensions;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

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
    public DbSet<SMChannelChannelLink> SMChannelChannelLinks { get; set; }
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

    public Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken = default)
    {
        return Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }

    public IQueryable<ReturnType> SqlQueryRaw<ReturnType>([NotParameterized] string sql, params object[] parameters)
    {
        return Database.SqlQueryRaw<ReturnType>(sql, parameters);
    }
    public Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    public void BulkUpdateEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        this.BulkUpdate(entities);
    }

    public void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        this.BulkInsert(entities);
    }

    public async Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        await this.BulkInsertAsync(entities);
    }

    public async Task BulkUpdateAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        await BulkUpdateAsync(entities);
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

        _ = modelBuilder.Entity<ChannelGroup>()
            .HasIndex(e => e.Name)
            .HasDatabaseName("idx_Name");

        _ = modelBuilder.Entity<SMChannel>()
            .HasIndex(e => e.Id)
            .HasDatabaseName("idx_smchannels_id")
            .IsUnique();

        _ = modelBuilder.Entity<SMChannel>()
            .HasIndex(e => e.BaseStreamID)
            .HasDatabaseName("idx_smchannels_basestreamid");

        _ = modelBuilder.Entity<SMChannel>()
            .HasIndex(e => e.Name)
            .HasDatabaseName("idx_SMChannelName");

        _ = modelBuilder.Entity<SMChannel>()
            .HasIndex(e => new { e.ChannelNumber, e.Id })
            .HasDatabaseName("idx_smchannels_channelnumber_id");

        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => e.Id)
            .HasDatabaseName("idx_smstreams_id")
            .IsUnique();

        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => e.Name)
            .HasDatabaseName("idx_SMStreamName");

        _ = modelBuilder.Entity<SMChannelStreamLink>()
            .HasIndex(e => new { e.SMChannelId, e.SMStreamId })
            .HasDatabaseName("idx_smchannelstreamlinks_smchannelid_smstreamid")
            .IsUnique();

        _ = modelBuilder.Entity<SMChannelStreamLink>()
            .HasIndex(e => new { e.SMChannelId, e.Rank })
            .HasDatabaseName("idx_smchannelstreamlinks_smchannelid_rank");

        _ = modelBuilder.Entity<StreamGroupSMChannelLink>()
            .HasIndex(e => new { e.SMChannelId, e.StreamGroupId })
            .HasDatabaseName("idx_streamgroupsmchannellink_smchannelid_streamgroupid")
            .IsUnique();

        _ = modelBuilder.Entity<StreamGroup>()
            .HasIndex(e => new { e.Name, e.Id })
            .HasDatabaseName("idx_streamgroups_name_id");

        _ = modelBuilder.Entity<ChannelGroup>()
            .HasIndex(e => new { e.Name, e.IsHidden })
            .HasDatabaseName("idx_Name_IsHidden");

        _ = modelBuilder.Entity<EPGFile>()
            .HasIndex(e => e.Url)
            .HasDatabaseName("idx_epgfiles_url");

        modelBuilder.ApplyUtcDateTimeConverter();

        base.OnModelCreating(modelBuilder);
    }
}
