using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsQuery(ChannelGroupParameters Parameters) : IRequest<PagedResponse<ChannelGroupDto>>;

internal class GetChannelGroupsQueryHandler : BaseMediatorRequestHandler, IRequestHandler<GetChannelGroupsQuery, PagedResponse<ChannelGroupDto>>
{
    public GetChannelGroupsQueryHandler(ILogger<GetChannelGroupsQuery> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, publisher, sender, hubContext) { }

    public async Task<PagedResponse<ChannelGroupDto>> Handle(GetChannelGroupsQuery request, CancellationToken cancellationToken)
    {

        int count = Repository.EPGFile.Count();

        if (request.Parameters.PageSize == 0)
        {
            PagedResponse<ChannelGroupDto> emptyResponse = new()
            {
                TotalItemCount = count
            };
            return Repository.ChannelGroup.CreateEmptyPagedResponse(request.Parameters);
        }

        return await Repository.ChannelGroup.GetChannelGroupsAsync(request.Parameters).ConfigureAwait(false);
    }
}