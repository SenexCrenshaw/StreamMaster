using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupNamesQuery() : IRequest<List<string>>;

internal class GetChannelGroupNamesQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupNamesQuery, List<string>>
{
    public GetChannelGroupNamesQueryHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<List<string>> Handle(GetChannelGroupNamesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<string> res = Repository.ChannelGroup.GetAllChannelGroupNames();
        return await res.ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}