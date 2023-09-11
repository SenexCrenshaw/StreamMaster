namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroup(int Id) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroup, ChannelGroupDto?>
{

    public GetChannelGroupHandler(ILogger<GetChannelGroup> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
 : base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }


    public async Task<ChannelGroupDto?> Handle(GetChannelGroup request, CancellationToken cancellationToken)
    {
        ChannelGroupDto? channelGroup = await Repository.ChannelGroup.GetChannelGroupAsync(request.Id);

        return channelGroup;
    }
}