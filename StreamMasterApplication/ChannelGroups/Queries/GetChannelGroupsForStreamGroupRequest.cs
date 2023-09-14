namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsForStreamGroupRequest(int StreamGroupId) : IRequest<List<ChannelGroupDto>>;



[LogExecutionTimeAspect]
public class GetChannelGroupsForStreamGroupRequestHandler(ILogger<GetChannelGroupsForStreamGroupRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetChannelGroupsForStreamGroupRequest, List<ChannelGroupDto>>
{
    public async Task<List<ChannelGroupDto>> Handle(GetChannelGroupsForStreamGroupRequest request, CancellationToken cancellationToken)
    {
        return await Repository.ChannelGroup.GetChannelGroupsForStreamGroup(request.StreamGroupId, cancellationToken);
    }

}