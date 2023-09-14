namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsFromVideoStreamIds(IEnumerable<string> VideoStreamIds) : IRequest<List<ChannelGroupDto>>;

internal class GetChannelGroupsFromVideoStreamIdsHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupsFromVideoStreamIds, List<ChannelGroupDto>>
{

    public GetChannelGroupsFromVideoStreamIdsHandler(ILogger<GetChannelGroupsFromVideoStreamIds> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task<List<ChannelGroupDto>> Handle(GetChannelGroupsFromVideoStreamIds request, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> res = await Repository.ChannelGroup.GetChannelGroupsFromVideoStreamIds(request.VideoStreamIds, cancellationToken).ConfigureAwait(false);
        return res;
    }
}