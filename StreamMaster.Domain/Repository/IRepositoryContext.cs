using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.Domain.Repository;

public interface IRepositoryContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    int ExecuteSqlRaw(string sql, params object[] parameters);
    Task<int> ExecuteSqlRawAsyncEntities(string sql, CancellationToken cancellationToken = default);
    void BulkUpdateEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    void BulkInsertEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    Task BulkDeleteAsyncEntities<TEntity>(IQueryable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class;
    DbSet<ChannelGroup> ChannelGroups
    { get; set; }
    DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    DbSet<EPGFile> EPGFiles { get; set; }
    DbSet<M3UFile> M3UFiles { get; set; }
    DbSet<StreamGroupChannelGroup> StreamGroupChannelGroups { get; set; }
    DbSet<StreamGroupSMChannelLink> StreamGroupSMChannelLinks { get; set; }
    DbSet<StreamGroup> StreamGroups { get; set; }
    DbSet<StreamGroupVideoStream> StreamGroupVideoStreams { get; set; }
    DbSet<SMStream> SMStreams { get; set; }
    DbSet<SMChannel> SMChannels { get; set; }
    DbSet<SystemKeyValue> SystemKeyValues { get; set; }
    DbSet<VideoStreamLink> VideoStreamLinks { get; set; }
    DbSet<VideoStream> VideoStreams { get; set; }

    void Dispose();
    bool IsEntityTracked<TEntity>(TEntity entity) where TEntity : class;
    abstract Task MigrateData(List<MxfService>? allServices = null);
}