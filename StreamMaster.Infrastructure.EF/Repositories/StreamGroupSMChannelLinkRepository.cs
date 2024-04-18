using AutoMapper;

using MediatR;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Exceptions;
namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupSMChannelLinkRepository(ILogger<StreamGroupSMChannelLinkRepository> logger, IRepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper, IOptionsMonitor<Setting> intSettings, ISender sender)
    : RepositoryBase<StreamGroupSMChannelLink>(repositoryContext, logger, intSettings), IStreamGroupSMChannelLinkRepository
{
    public async Task<APIResponse> AddSMChannelToStreamGroup(int StreamGroupId, int SMChannelId)
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
            StreamGroupId = StreamGroupId
        };

        Create(link);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task CreateStreamGroupSMChannelLink(int StreamGroupId, int SMChannelId)
    {
        if (Any(a => a.StreamGroupId == StreamGroupId && a.SMChannelId == SMChannelId))
        {
            return;
        }


        StreamGroupSMChannelLink link = new()
        {
            SMChannelId = SMChannelId,
            StreamGroupId = StreamGroupId,
            IsReadOnly = false,
            Rank = 0
        };

        Create(link);
        await SaveChangesAsync();

    }

    public async Task<APIResponse> RemoveSMChannelFromStreamGroup(int StreamGroupId, int SMChannelId)
    {
        IQueryable<StreamGroupSMChannelLink> toDelete = GetQuery(true).Where(a => a.SMChannelId == SMChannelId && a.StreamGroupId == StreamGroupId);
        if (!toDelete.Any())
        {
            throw new APIException($"Channel with Id {SMChannelId} or Stream Group with Id {StreamGroupId} not found");
        }

        await BulkDeleteAsync(toDelete);
        //List<int> smchannelIds = toDelete.Select(a => a.SMChannelId).ToList();
        //foreach (StreamGroupSMChannelLink link in toDelete)
        //{
        //    Delete(link);
        //}
        await SaveChangesAsync();

        return APIResponse.Success;
    }
}