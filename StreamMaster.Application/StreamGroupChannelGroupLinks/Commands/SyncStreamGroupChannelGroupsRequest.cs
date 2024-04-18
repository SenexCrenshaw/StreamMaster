namespace StreamMaster.Application.StreamGroupChannelGroupLinks.Commands;

public record SyncStreamGroupChannelGroupsRequest(int StreamGroupId, List<int> ChannelGroupIds) : IRequest<StreamGroupDto?>;

[LogExecutionTimeAspect]
internal class SyncStreamGroupChannelGroupsRequestHandler(ILogger<SyncStreamGroupChannelGroupsRequest> logger, IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<SyncStreamGroupChannelGroupsRequest, StreamGroupDto?>
{
    public async Task<StreamGroupDto?> Handle(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken = default)
    {
        StreamGroupDto? ret = await Repository.StreamGroupChannelGroup.SyncStreamGroupChannelGroups(request.StreamGroupId, request.ChannelGroupIds).ConfigureAwait(false);

        if (ret != null)
        {

            await HubContext.Clients.All.StreamGroupsRefresh([ret]).ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupChannelGroupsRefresh().ConfigureAwait(false);
        }
        return ret;

    }
}
