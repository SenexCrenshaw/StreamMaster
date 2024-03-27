namespace StreamMaster.Application.ChannelGroups.Queries;

public record GetChannelGroupNames() : IRequest<IEnumerable<string>>;

internal class GetChannelGroupNamesQueryHandler(ILogger<GetChannelGroupNames> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetChannelGroupNames, IEnumerable<string>>
{
    public async Task<IEnumerable<string>> Handle(GetChannelGroupNames request, CancellationToken cancellationToken)
    {
        List<ChannelGroupIdName> ret = await Repository.ChannelGroup.GetChannelGroupNames(cancellationToken).ConfigureAwait(false);
        int test = ret.Count;
        return ret.Select(a => a.Name);
    }
}