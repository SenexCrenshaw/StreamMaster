namespace StreamMasterApplication.StreamGroupChannelGroups.Commands;

public record SyncStreamGroupChannelGroupsRequest(int StreamGroupId, List<int> ChannelGroupIds) : IRequest<StreamGroupDto?>;

internal class SyncStreamGroupChannelGroupsRequestHandler(ILogger<SyncStreamGroupChannelGroupsRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<SyncStreamGroupChannelGroupsRequest, StreamGroupDto?>
{
    public async Task<StreamGroupDto?> Handle(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken = default)
    {
        StreamGroupDto? ret = await Repository.StreamGroupChannelGroup.SyncStreamGroupChannelGroups(request.StreamGroupId, request.ChannelGroupIds, cancellationToken).ConfigureAwait(false);

        if (ret != null)
        {
            //await HubContext.Clients.All.VideoStreamsRefresh(ret).ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupsRefresh([ret]).ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupChannelGroupsRefresh().ConfigureAwait(false);
        }
        return ret;

    }
}
