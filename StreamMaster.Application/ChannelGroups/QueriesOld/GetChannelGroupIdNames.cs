namespace StreamMaster.Application.ChannelGroups.QueriesOld;

public record GetChannelGroupIdNames() : IRequest<List<ChannelGroupIdName>>;

internal class GetChannelGroupIdNamesQueryHandler(ILogger<GetChannelGroupIdNames> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetChannelGroupIdNames, List<ChannelGroupIdName>>
{
    public async Task<List<ChannelGroupIdName>> Handle(GetChannelGroupIdNames request, CancellationToken cancellationToken)
    {
        List<ChannelGroupIdName> ret = await Repository.ChannelGroup.GetChannelGroupNames(cancellationToken);

        return ret;
    }
}