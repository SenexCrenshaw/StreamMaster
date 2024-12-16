using System.Text;

using Microsoft.EntityFrameworkCore;

using Npgsql;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Exceptions;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupSMChannelLinkRepository(ILogger<StreamGroupSMChannelLinkRepository> logger, IRepositoryContext repositoryContext, IRepositoryWrapper repository)
    : RepositoryBase<StreamGroupSMChannelLink>(repositoryContext, logger), IStreamGroupSMChannelLinkRepository
{
    public async Task AddSMChannelsToStreamGroupAsync(int streamGroupId, List<int> smChannelIds, bool? skipSave = false)
    {
        if (smChannelIds == null || smChannelIds.Count == 0)
        {
            return;
        }

        const int batchSize = 50;
        // Break the smChannelIds list into batches
        for (int i = 0; i < smChannelIds.Count; i += batchSize)
        {
            List<int> batch = [.. smChannelIds.Skip(i).Take(batchSize)];
            await InsertBatchAsync(streamGroupId, batch);
        }

        if (skipSave == false)
        {
            await SaveChangesAsync();
        }
    }

    private async Task InsertBatchAsync(int streamGroupId, List<int> smChannelIds)
    {
        if (smChannelIds == null || smChannelIds.Count == 0)
        {
            return;
        }

        StringBuilder sqlBuilder = new();
        sqlBuilder.Append("INSERT INTO public.\"StreamGroupSMChannelLink\" (\"SMChannelId\", \"StreamGroupId\", \"IsReadOnly\", \"Rank\") VALUES ");

        List<NpgsqlParameter> parameters = [];

        for (int i = 0; i < smChannelIds.Count; i++)
        {
            int smChannelId = smChannelIds[i];
            NpgsqlParameter paramSmChannelId = new($"@smChannelId{i}", smChannelId);
            NpgsqlParameter paramStreamGroupId = new($"@streamGroupId{i}", streamGroupId);
            sqlBuilder.Append($"({paramSmChannelId.ParameterName}, {paramStreamGroupId.ParameterName}, false, 0)");

            if (i < smChannelIds.Count - 1)
            {
                sqlBuilder.Append(", ");
            }

            parameters.Add(paramSmChannelId);
            parameters.Add(paramStreamGroupId);
        }

        sqlBuilder.Append(" ON CONFLICT (\"SMChannelId\", \"StreamGroupId\") DO NOTHING;");

        await using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await RepositoryContext.BeginTransactionAsync();
        try
        {
            await RepositoryContext.ExecuteSqlRawAsync(sqlBuilder.ToString(), [.. parameters]);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<APIResponse> AddSMChannelToStreamGroup(int StreamGroupId, int SMChannelId, bool? skipSave = false, bool? skipCheck = false)
    {
        if (repository.StreamGroup.GetStreamGroup(StreamGroupId) == null || repository.SMChannel.GetSMChannel(SMChannelId) == null)
        {
            throw new APIException($"Channel with Id {SMChannelId} or Stream Group with Id {StreamGroupId} not found");
        }

        if (skipCheck == false)
        {
            if (Any(a => a.StreamGroupId == StreamGroupId && a.SMChannelId == SMChannelId))
            {
                return APIResponse.Success;
            }
        }

        StreamGroupSMChannelLink link = new()
        {
            SMChannelId = SMChannelId,
            StreamGroupId = StreamGroupId,
            IsReadOnly = false,
            Rank = 0
        };

        Create(link);
        if (skipSave == false)
        {
            await SaveChangesAsync();
        }

        return APIResponse.Success;
    }

    public async Task<APIResponse> RemoveSMChannelFromStreamGroup(int StreamGroupId, int SMChannelId)
    {
        IQueryable<StreamGroupSMChannelLink> toDelete = GetQuery(true).Where(a => a.SMChannelId == SMChannelId && a.StreamGroupId == StreamGroupId);
        if (!toDelete.Any())
        {
            throw new APIException($"Channel with Id {SMChannelId} or Stream Group with Id {StreamGroupId} not found");
        }

        await BulkDeleteAsync(toDelete);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> RemoveSMChannelsFromStreamGroup(int StreamGroupId, List<int> SMChannelIds)
    {
        IQueryable<StreamGroupSMChannelLink> toDelete = GetQuery(true).Where(a => SMChannelIds.Contains(a.SMChannelId) && a.StreamGroupId == StreamGroupId);
        if (!toDelete.Any())
        {
            return APIResponse.Success;
        }

        await BulkDeleteAsync(toDelete);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public IQueryable<StreamGroupSMChannelLink> GetQueryNoTracking => base.GetQuery(false).Include(a => a.SMChannel).AsNoTracking();

    public override IQueryable<StreamGroupSMChannelLink> GetQuery(bool tracking = false)
    {
        return tracking
            ? base.GetQuery(tracking).Include(a => a.SMChannel)
            : base.GetQuery(tracking).Include(a => a.SMChannel).AsNoTracking();
    }
}