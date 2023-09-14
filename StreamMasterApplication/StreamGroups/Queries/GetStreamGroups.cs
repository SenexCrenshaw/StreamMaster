using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetStreamGroups(StreamGroupParameters Parameters) : IRequest<PagedResponse<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupsHandler : BaseMediatorRequestHandler, IRequestHandler<GetStreamGroups, PagedResponse<StreamGroupDto>>
{

    public GetStreamGroupsHandler(ILogger<GetStreamGroups> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }
    public async Task<PagedResponse<StreamGroupDto>> Handle(GetStreamGroups request, CancellationToken cancellationToken = default)
    {
        if (request.Parameters.PageSize == 0)
        {
            return request.Parameters.CreateEmptyPagedResponse<StreamGroupDto>();
        }

        return await Repository.StreamGroup.GetPagedStreamGroups(request.Parameters).ConfigureAwait(false);

    }
}