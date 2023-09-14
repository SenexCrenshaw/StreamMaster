namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupByName(string Name) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupByNameHandler(ILogger<GetChannelGroupByName> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetChannelGroupByName, ChannelGroupDto?>
{
    public async Task<ChannelGroupDto?> Handle(GetChannelGroupByName request, CancellationToken cancellationToken)
    {
        ChannelGroupDto? channelGroup = await Repository.ChannelGroup.GetChannelGroupByName(request.Name).ConfigureAwait(false);
        return channelGroup;
    }
}
