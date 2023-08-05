using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

using StreamMasterApplication.M3UFiles.Commands;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsQuery(ChannelGroupParameters Parameters) : IRequest<PagedList<ChannelGroup>>;

internal class GetChannelGroupsQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupsQuery, PagedList<ChannelGroup>>
{

    public GetChannelGroupsQueryHandler(ILogger<CreateM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender)
        : base(logger, repository, mapper, publisher, sender) { }

    public async Task<PagedList<ChannelGroup>> Handle(GetChannelGroupsQuery request, CancellationToken cancellationToken)
    {
        return await Repository.ChannelGroup.GetChannelGroupsAsync(request.Parameters).ConfigureAwait(false);

    }
}
