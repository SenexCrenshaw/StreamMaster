namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupFromVideoStreamId(string VideoStreamId) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupFromVideoStreamIdHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupFromVideoStreamId, ChannelGroupDto?>
{

    public GetChannelGroupFromVideoStreamIdHandler(ILogger<GetChannelGroupFromVideoStreamId> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
 : base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task<ChannelGroupDto?> Handle(GetChannelGroupFromVideoStreamId request, CancellationToken cancellationToken)
    {
        ChannelGroupDto? res = await Repository.ChannelGroup.GetChannelGroupFromVideoStreamId(request.VideoStreamId).ConfigureAwait(false);
        return res;
    }
}