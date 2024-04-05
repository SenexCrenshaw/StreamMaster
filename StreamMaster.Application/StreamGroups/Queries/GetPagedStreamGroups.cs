namespace StreamMaster.Application.StreamGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedStreamGroupsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetPagedStreamGroupsRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetPagedStreamGroupsRequest, PagedResponse<StreamGroupDto>>
{
    public async Task<PagedResponse<StreamGroupDto>> Handle(GetPagedStreamGroupsRequest request, CancellationToken cancellationToken = default)
    {

        return request.Parameters.PageSize == 0
            ? Repository.StreamGroup.CreateEmptyPagedResponse()
            : await Repository.StreamGroup.GetPagedStreamGroups(request.Parameters).ConfigureAwait(false);
    }
}