namespace StreamMaster.Application.StreamGroupChannelGroupLinks.Commands;

public record SyncStreamGroupChannelGroupByChannelIdRequest(int ChannelGroupId) : IRequest;

[LogExecutionTimeAspect]
internal class SyncStreamGroupChannelGroupsByChannelIdsRequestHandler(ILogger<SyncStreamGroupChannelGroupByChannelIdRequest> logger, IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<SyncStreamGroupChannelGroupByChannelIdRequest>
{
    public async Task Handle(SyncStreamGroupChannelGroupByChannelIdRequest request, CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto>? ret = await Repository.StreamGroupChannelGroup.SyncStreamGroupChannelGroupByChannelId(request.ChannelGroupId).ConfigureAwait(false);

        if (ret != null)
        {

            await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupChannelGroupsRefresh().ConfigureAwait(false);
        }
        return;
    }
}
