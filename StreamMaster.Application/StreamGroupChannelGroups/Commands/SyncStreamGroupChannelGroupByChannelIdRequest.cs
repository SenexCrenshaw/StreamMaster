namespace StreamMaster.Application.StreamGroupChannelGroups.Commands;

public record SyncStreamGroupChannelGroupByChannelIdRequest(int ChannelGroupId) : IRequest;

[LogExecutionTimeAspect]
internal class SyncStreamGroupChannelGroupsByChannelIdsRequestHandler(ILogger<SyncStreamGroupChannelGroupByChannelIdRequest> logger, IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<SyncStreamGroupChannelGroupByChannelIdRequest>
{
    public async Task Handle(SyncStreamGroupChannelGroupByChannelIdRequest request, CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto>? ret = await Repository.StreamGroupChannelGroup.SyncStreamGroupChannelGroupByChannelIdRequest(request.ChannelGroupId, cancellationToken).ConfigureAwait(false);

        if (ret != null)
        {

            //await HubContext.Clients.All.StreamGroupsRefresh([.. ret]).ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupVideoStreamsRefresh().ConfigureAwait(false);
            await HubContext.Clients.All.StreamGroupChannelGroupsRefresh().ConfigureAwait(false);
        }
        return;
    }
}
