using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups.Queries;

public record GetChannelGroupsQuery(ChannelGroupParameters Parameters) : IRequest<PagedResponse<ChannelGroupDto>>;


[LogExecutionTimeAspect]
internal class GetChannelGroupsQueryHandler(ILogger<GetChannelGroupsQuery> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetChannelGroupsQuery, PagedResponse<ChannelGroupDto>>
{
    public async Task<PagedResponse<ChannelGroupDto>> Handle(GetChannelGroupsQuery request, CancellationToken cancellationToken)
    {
        if (request.Parameters.PageSize == 0)
        {
            return request.Parameters.CreateEmptyPagedResponse<ChannelGroupDto>();
        }

        return await Repository.ChannelGroup.GetPagedChannelGroups(request.Parameters).ConfigureAwait(false);
    }
}