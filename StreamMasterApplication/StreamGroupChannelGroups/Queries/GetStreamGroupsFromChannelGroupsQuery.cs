namespace StreamMasterApplication.StreamGroupChannelGroups.Queries;

public record GetStreamGroupsFromChannelGroupsQuery(List<int> channelGroupIds) : IRequest<IEnumerable<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupsFromChannelGroupsQueryHandler(ILogger<GetStreamGroupsFromChannelGroupsQuery> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetStreamGroupsFromChannelGroupsQuery, IEnumerable<StreamGroupDto>>
{
    public async Task<IEnumerable<StreamGroupDto>> Handle(GetStreamGroupsFromChannelGroupsQuery request, CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto> ret = await Repository.StreamGroupChannelGroup.GetStreamGroupsFromChannelGroups(request.channelGroupIds, cancellationToken).ConfigureAwait(false);

        return ret;

    }
}
