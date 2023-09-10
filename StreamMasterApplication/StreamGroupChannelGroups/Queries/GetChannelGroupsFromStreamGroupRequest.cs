namespace StreamMasterApplication.StreamGroupChannelGroups.Queries;

public record GetChannelGroupsFromStreamGroupRequest(int StreamGroupId) : IRequest<IEnumerable<ChannelGroupDto>>;

[LogExecutionTimeAspect]
internal class GetChannelGroupsFromStreamGroupRequestHandler(ILogger<GetChannelGroupsFromStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<GetChannelGroupsFromStreamGroupRequest, IEnumerable<ChannelGroupDto>>
{
    public async Task<IEnumerable<ChannelGroupDto>> Handle(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ChannelGroupDto> ret = await Repository.StreamGroupChannelGroup.GetChannelGroupsFromStreamGroup(request.StreamGroupId, cancellationToken).ConfigureAwait(false);

        return ret;

    }
}
