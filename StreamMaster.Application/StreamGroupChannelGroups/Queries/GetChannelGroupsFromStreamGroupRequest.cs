namespace StreamMaster.Application.StreamGroupChannelGroups.Queries;

public record GetChannelGroupsFromStreamGroupRequest(int StreamGroupId) : IRequest<IEnumerable<ChannelGroupDto>>;

[LogExecutionTimeAspect]
internal class GetChannelGroupsFromStreamGroupRequestHandler(ILogger<GetChannelGroupsFromStreamGroupRequest> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetChannelGroupsFromStreamGroupRequest, IEnumerable<ChannelGroupDto>>
{
    public async Task<IEnumerable<ChannelGroupDto>> Handle(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ChannelGroupDto> ret = await Repository.StreamGroupChannelGroup.GetChannelGroupsFromStreamGroup(request.StreamGroupId).ConfigureAwait(false);

        return ret;

    }
}
