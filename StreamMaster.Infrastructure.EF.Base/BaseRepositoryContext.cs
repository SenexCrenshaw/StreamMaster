using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace StreamMaster.Infrastructure.EF.Base;
#pragma warning disable CS8618 
public class BaseRepositoryContext(DbContextOptions options)
    : DbContext(options), IDataProtectionKeyContext, IRepositoryContext
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
#pragma warning restore CS8618
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
    public async Task BulkUpdateEntitiesAsync<TEntity>(
    List<TEntity> entities,
    int batchSize = 100,
    int maxDegreeOfParallelism = 4,
    CancellationToken cancellationToken = default
) where TEntity : class
    {
        if (entities == null || entities.Count == 0)
        {
            throw new ArgumentNullException(nameof(entities));
        }

        // Split entities into batches
        List<List<TEntity>> batches = [.. entities
            .Select((entity, index) => new { entity, index })
            .GroupBy(x => x.index / batchSize)
            .Select(g => g.Select(x => x.entity).ToList())];

        // Limit the number of parallel tasks to avoid overwhelming the database
        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        List<Task> updateTasks = [];

        // Execute updates in parallel
        await Task.Run(() => Parallel.ForEach(batches, parallelOptions, batch => updateTasks.Add(UpdateBatchAsync(batch, cancellationToken))));

        // Wait for all updates to complete
        await Task.WhenAll(updateTasks);
    }

    private async Task UpdateBatchAsync<TEntity>(List<TEntity> batch, CancellationToken cancellationToken) where TEntity : class
    {
        foreach (TEntity entity in batch)
        {
            Set<TEntity>().Attach(entity);
            Entry(entity).State = EntityState.Modified;
        }

        await SaveChangesAsync(cancellationToken);
    }

    //public void BulkUpdateEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    //{
    //    this.BulkUpdate(entities);
    //}

    //public void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    //{
    //    this.BulkInsert(entities);
    //}

    //public async Task BulkInsertEntitiesAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    //{
    //    await this.BulkInsertAsync(entities);
    //}

    public async Task BulkUpdateAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        await BulkUpdateAsync(entities);
    }
    public Task BulkDeleteAsyncEntities<TEntity>(IQueryable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class
    {
        return entities.ExecuteDeleteAsync(cancellationToken);
        //return Database.ExecuteDeleteAsync(entities, cancellationToken);
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
                base.Dispose();
            }
        }
        _disposed = true;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityAlwaysColumns();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseRepositoryContext).Assembly);

        // Apply configurations

        //ConfigureChannelGroup(modelBuilder);

        //ConfigureSMChannel(modelBuilder);

        //ConfigureSMStream(modelBuilder);

        //ConfigureSMChannelStreamLink(modelBuilder);

        //ConfigureStreamGroup(modelBuilder);

        //ConfigureStreamGroupSMChannelLink(modelBuilder);

        //ConfigureEPGFile(modelBuilder);

        // Ensure UTC DateTime Conversion
        modelBuilder.ApplyUtcDateTimeConverter();

        base.OnModelCreating(modelBuilder);
    }

    #region ChannelGroup Configuration
    private static void ConfigureChannelGroup(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<ChannelGroup>()
            .HasIndex(e => e.Name)
            .HasDatabaseName("idx_Name");

        _ = modelBuilder.Entity<ChannelGroup>()
            .HasIndex(e => new { e.Name, e.IsHidden })
            .HasDatabaseName("idx_Name_IsHidden");
    }
    #endregion

    #region SMChannel Configuration
    private static void ConfigureSMChannel(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<SMChannel>()
            .HasIndex(e => e.Id)
            .HasDatabaseName("idx_smchannels_id")
            .IsUnique();

        _ = modelBuilder.Entity<SMChannel>()
            .HasIndex(e => e.Group)
            .HasDatabaseName("idx_smchannels_group");

        _ = modelBuilder.Entity<SMChannel>()
            .HasIndex(e => e.BaseStreamID)
            .HasDatabaseName("idx_smchannels_basestreamid");

        _ = modelBuilder.Entity<SMChannel>()
            .HasIndex(e => e.Name)
            .HasDatabaseName("idx_SMChannelName");

        _ = modelBuilder.Entity<SMChannel>()
            .HasIndex(e => new { e.ChannelNumber, e.Id })
            .HasDatabaseName("idx_smchannels_channelnumber_id");
    }
    #endregion

    #region SMStream Configuration
    private static void ConfigureSMStream(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => e.Name)
            .HasDatabaseName("idx_smstreams_name");

        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => e.Id)
            .HasDatabaseName("idx_smstreams_id")
            .IsUnique();

        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => e.Name)
            .HasDatabaseName("idx_SMStreamName");

        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => e.Group)
            .HasDatabaseName("idx_smstreams_group");

        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => new { e.Group, e.IsHidden })
            .HasDatabaseName("idx_smstreams_group_ishidden");

        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => e.M3UFileId)
            .HasDatabaseName("idx_smstreams_m3ufileid");

        _ = modelBuilder.Entity<SMStream>()
            .HasIndex(e => new { e.NeedsDelete, e.M3UFileId })
            .HasDatabaseName("idx_smstreams_needsdelete_m3ufileid");
    }
    #endregion

    #region SMChannelStreamLink Configuration
    private static void ConfigureSMChannelStreamLink(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<SMChannelStreamLink>()
            .HasIndex(e => new { e.SMChannelId, e.SMStreamId })
            .HasDatabaseName("idx_smchannelstreamlinks_smchannelid_smstreamid")
            .IsUnique();

        _ = modelBuilder.Entity<SMChannelStreamLink>()
            .HasIndex(e => new { e.SMChannelId, e.Rank })
            .HasDatabaseName("idx_smchannelstreamlinks_smchannelid_rank");
    }
    #endregion

    #region StreamGroup Configuration
    private static void ConfigureStreamGroup(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<StreamGroup>()
            .HasIndex(e => new { e.Name, e.Id })
            .HasDatabaseName("idx_streamgroups_name_id");
    }
    #endregion

    #region StreamGroupSMChannelLink Configuration
    private static void ConfigureStreamGroupSMChannelLink(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<StreamGroupSMChannelLink>()
            .HasIndex(e => new { e.SMChannelId, e.StreamGroupId })
            .HasDatabaseName("idx_streamgroupsmchannellink_smchannelid_streamgroupid")
            .IsUnique();
    }
    #endregion

    #region EPGFile Configuration
    private static void ConfigureEPGFile(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.Entity<EPGFile>()
            .HasIndex(e => e.Url)
            .HasDatabaseName("idx_epgfiles_url");
    }
    #endregion

}
