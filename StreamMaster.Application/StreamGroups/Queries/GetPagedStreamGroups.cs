using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.StreamGroups.Queries;

public record GetPagedStreamGroups(StreamGroupParameters Parameters) : IRequest<PagedResponse<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetPagedStreamGroupsHandler(ILogger<GetPagedStreamGroups> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetPagedStreamGroups, PagedResponse<StreamGroupDto>>
{
    public async Task<PagedResponse<StreamGroupDto>> Handle(GetPagedStreamGroups request, CancellationToken cancellationToken = default)
    {
        return request.Parameters.PageSize == 0
            ? Repository.StreamGroup.CreateEmptyPagedResponse()
            : await Repository.StreamGroup.GetPagedStreamGroups(request.Parameters).ConfigureAwait(false);
    }
}