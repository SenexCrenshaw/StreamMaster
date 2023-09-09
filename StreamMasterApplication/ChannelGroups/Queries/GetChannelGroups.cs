using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsQuery(ChannelGroupParameters Parameters) : IRequest<PagedResponse<ChannelGroupDto>>;

internal class GetChannelGroupsQueryHandler(ILogger<GetChannelGroupsQuery> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : BaseMediatorRequestHandler(logger, repository, mapper, publisher, sender, hubContext), IRequestHandler<GetChannelGroupsQuery, PagedResponse<ChannelGroupDto>>
{
    public async Task<PagedResponse<ChannelGroupDto>> Handle(GetChannelGroupsQuery request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.ChannelGroup.CreateEmptyPagedResponse(request.Parameters);
        }

        return await Repository.ChannelGroup.GetChannelGroupsAsync(request.Parameters).ConfigureAwait(false);
    }
}