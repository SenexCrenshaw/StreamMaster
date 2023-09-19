using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroups.Queries;

public record GetPagedStreamGroups(StreamGroupParameters Parameters) : IRequest<PagedResponse<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetPagedStreamGroupsHandler(ILogger<GetPagedStreamGroups> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetPagedStreamGroups, PagedResponse<StreamGroupDto>>
{
    public async Task<PagedResponse<StreamGroupDto>> Handle(GetPagedStreamGroups request, CancellationToken cancellationToken = default)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.StreamGroup.CreateEmptyPagedResponse();
        }

        return await Repository.StreamGroup.GetPagedStreamGroups(request.Parameters).ConfigureAwait(false);

    }
}