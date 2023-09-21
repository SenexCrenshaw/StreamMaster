namespace StreamMasterApplication.StreamGroupChannelGroups.Queries;

public record GetChannelGroupsFromStreamGroupRequest(int StreamGroupId) : IRequest<IEnumerable<ChannelGroupDto>>;

[LogExecutionTimeAspect]
internal class GetChannelGroupsFromStreamGroupRequestHandler(ILogger<GetChannelGroupsFromStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetChannelGroupsFromStreamGroupRequest, IEnumerable<ChannelGroupDto>>
{
    public async Task<IEnumerable<ChannelGroupDto>> Handle(GetChannelGroupsFromStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        IEnumerable<ChannelGroupDto> ret = await Repository.StreamGroupChannelGroup.GetChannelGroupsFromStreamGroup(request.StreamGroupId, cancellationToken).ConfigureAwait(false);

        return ret;

    }
}
