namespace StreamMasterApplication.StreamGroupChannelGroups.Commands;

public record RemoveStreamGroupChannelGroupsRequest(int StreamGroupId, List<int> ChannelGroupIds) : IRequest<IEnumerable<string>>;

internal class RemoveStreamGroupChannelGroupsRequestHandler(ILogger<RemoveStreamGroupChannelGroupsRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<RemoveStreamGroupChannelGroupsRequest, IEnumerable<string>>
{
    public async Task<IEnumerable<string>> Handle(RemoveStreamGroupChannelGroupsRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<string> ret = await Repository.StreamGroupChannelGroup.RemoveStreamGroupChannelGroups(request.StreamGroupId, request.ChannelGroupIds, cancellationToken).ConfigureAwait(false);
        return ret;

    }
}
