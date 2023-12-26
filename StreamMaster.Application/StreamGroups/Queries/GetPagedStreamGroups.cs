using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.StreamGroups.Queries;

public record GetPagedStreamGroups(StreamGroupParameters Parameters) : IRequest<PagedResponse<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetPagedStreamGroupsHandler(ILogger<GetPagedStreamGroups> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetPagedStreamGroups, PagedResponse<StreamGroupDto>>
{
    public async Task<PagedResponse<StreamGroupDto>> Handle(GetPagedStreamGroups request, CancellationToken cancellationToken = default)
    {
        return request.Parameters.PageSize == 0
            ? Repository.StreamGroup.CreateEmptyPagedResponse()
            : await Repository.StreamGroup.GetPagedStreamGroups(request.Parameters).ConfigureAwait(false);
    }
}