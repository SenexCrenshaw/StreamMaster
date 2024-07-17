using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Exceptions;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupSMChannelLinkRepository(ILogger<StreamGroupSMChannelLinkRepository> logger, IRepositoryContext repositoryContext, IRepositoryWrapper repository)
    : RepositoryBase<StreamGroupSMChannelLink>(repositoryContext, logger), IStreamGroupSMChannelLinkRepository
{
    public async Task<APIResponse> AddSMChannelToStreamGroup(int StreamGroupId, int SMChannelId, bool? skipSave = false)
    {
        if (repository.StreamGroup.GetStreamGroup(StreamGroupId) == null || repository.SMChannel.GetSMChannel(SMChannelId) == null)
        {
            throw new APIException($"Channel with Id {SMChannelId} or Stream Group with Id {StreamGroupId} not found");
        }

        if (Any(a => a.StreamGroupId == StreamGroupId && a.SMChannelId == SMChannelId))
        {
            return APIResponse.Success;
        }

        StreamGroupSMChannelLink link = new()
        {
            SMChannelId = SMChannelId,
            StreamGroupId = StreamGroupId,
            IsReadOnly = false,
            Rank = 0
        };

        Create(link);
        if (skipSave.HasValue && skipSave.Value == false)
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

    public override IQueryable<StreamGroupSMChannelLink> GetQuery(bool tracking = false)
    {
        return tracking
            ? base.GetQuery(tracking).Include(a => a.SMChannel)
            : base.GetQuery(tracking).Include(a => a.SMChannel).AsNoTracking();
    }

    public async Task AddSMChannelsToStreamGroup(int StreamGroupId, List<int> SMChannelIds)
    {
        foreach (var smChannelId in SMChannelIds)
        {
            await AddSMChannelToStreamGroup(StreamGroupId, smChannelId, true);
        }
    }
}