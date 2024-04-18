namespace StreamMaster.Application.StreamGroupChannelGroupLinks.Queries;

public record GetStreamGroupsFromChannelGroupQuery(int channelGroupId) : IRequest<IEnumerable<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupsFromChannelGroupQueryHandler(ILogger<GetStreamGroupsFromChannelGroupQuery> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetStreamGroupsFromChannelGroupQuery, IEnumerable<StreamGroupDto>>
{
    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroupsFromChannelGroupQuery request, CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto> ret = await Repository.StreamGroupChannelGroup.GetStreamGroupsFromChannelGroup(request.channelGroupId).ConfigureAwait(false);

        return ret;

    }
}
