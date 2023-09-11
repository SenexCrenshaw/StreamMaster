namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupIdNames() : IRequest<IEnumerable<ChannelGroupIdName>>;

internal class GetChannelGroupIdNamesQueryHandler(ILogger<GetChannelGroupIdNames> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper,settingsService, publisher, sender, hubContext), IRequestHandler<GetChannelGroupIdNames, IEnumerable<ChannelGroupIdName>>
{
    public Task<IEnumerable<ChannelGroupIdName>> Handle(GetChannelGroupIdNames request, CancellationToken cancellationToken)
    {
        IEnumerable<ChannelGroupIdName> ret = Repository.ChannelGroup.GetAllChannelGroupNames();
        return Task.FromResult(ret);
    }
}