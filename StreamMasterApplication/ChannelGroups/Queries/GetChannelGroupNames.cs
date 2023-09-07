using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupNames() : IRequest<List<string>>;

internal class GetChannelGroupNamesQueryHandler(ILogger<GetChannelGroupNames> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<GetChannelGroupNames, List<string>>
{
    public async Task<List<string>> Handle(GetChannelGroupNames request, CancellationToken cancellationToken)
    {
        IQueryable<string> res = Repository.ChannelGroup.GetAllChannelGroupNames();
        return await res.ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}