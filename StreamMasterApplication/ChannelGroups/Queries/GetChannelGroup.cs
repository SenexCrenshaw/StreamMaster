namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroup(int Id) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupHandler(ILogger<GetChannelGroup> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetChannelGroup, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(GetChannelGroup request, CancellationToken cancellationToken)
    {
        ChannelGroupDto? channelGroup = await Repository.ChannelGroup.GetChannelGroupById(request.Id);

        return channelGroup;
    }
}