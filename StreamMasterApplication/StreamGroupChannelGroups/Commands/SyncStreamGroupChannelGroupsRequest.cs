namespace StreamMasterApplication.StreamGroupChannelGroups.Commands;

public record SyncStreamGroupChannelGroupsRequest(int StreamGroupId, List<int> ChannelGroupIds) : IRequest<int>;

internal class SyncStreamGroupChannelGroupsRequestHandler(ILogger<SyncStreamGroupChannelGroupsRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<SyncStreamGroupChannelGroupsRequest, int>
{
    public async Task<int> Handle(SyncStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken = default)
    {
        int ret = await Repository.StreamGroupChannelGroup.SyncStreamGroupChannelGroups(request.StreamGroupId, request.ChannelGroupIds, cancellationToken).ConfigureAwait(false);

        if (ret > 0)
        {
            //await HubContext.Clients.All.VideoStreamsRefresh(ret).ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
        }
        return ret;

    }
}
