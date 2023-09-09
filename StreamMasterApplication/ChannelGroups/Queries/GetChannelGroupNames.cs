namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupNames() : IRequest<IEnumerable<string>>;

internal class GetChannelGroupNamesQueryHandler(ILogger<GetChannelGroupNames> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<GetChannelGroupNames, IEnumerable<string>>
{
    public Task<IEnumerable<string>> Handle(GetChannelGroupNames request, CancellationToken cancellationToken)
    {
        IEnumerable<ChannelGroupIdName> ret = Repository.ChannelGroup.GetAllChannelGroupNames();
        return Task.FromResult(ret.Select(a => a.Name));
    }
}