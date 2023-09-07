namespace StreamMasterApplication.StreamGroupChannelGroups.Queries;

public record GetStreamGroupsFromChannelGroupsQuery(List<int> channelGroupIds) : IRequest<IEnumerable<StreamGroupDto>>;

internal class GetStreamGroupsFromChannelGroupsQueryHandler(ILogger<GetStreamGroupsFromChannelGroupsQuery> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<GetStreamGroupsFromChannelGroupsQuery, IEnumerable<StreamGroupDto>>
{
    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroupsFromChannelGroupsQuery request, CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto> ret = await Repository.StreamGroupChannelGroup.GetStreamGroupsFromChannelGroups(request.channelGroupIds, cancellationToken).ConfigureAwait(false);

        return ret;

    }
}
