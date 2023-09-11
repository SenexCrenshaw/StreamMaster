namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupByName(string Name) : IRequest<ChannelGroupDto?>;

internal class GetChannelGroupByNameHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupByName, ChannelGroupDto?>
{

    public GetChannelGroupByNameHandler(ILogger<GetChannelGroupByName> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper,settingsService, publisher, sender, hubContext) { }


    public async Task<ChannelGroupDto?> Handle(GetChannelGroupByName request, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await Repository.ChannelGroup.GetChannelGroupByNameAsync(request.Name).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return null;
        }

        ChannelGroupDto ret = Mapper.Map<ChannelGroupDto>(channelGroup);

        return ret;
    }
}
