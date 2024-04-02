namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedStreamGroupsRequest(QueryStringParameters Parameters) : IRequest<APIResponse<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetPagedStreamGroupsRequestHandler(ILogger<GetPagedStreamGroupsRequest> logger, IRepositoryWrapper Repository)
    : IRequestHandler<GetPagedStreamGroupsRequest, APIResponse<StreamGroupDto>>
{
    public async Task<APIResponse<StreamGroupDto>> Handle(GetPagedStreamGroupsRequest request, CancellationToken cancellationToken = default)
    {
        APIResponse<StreamGroupDto> ret = new();
        if (request.Parameters.PageSize == 0)
        {
            ret.PagedResponse = Repository.StreamGroup.CreateEmptyPagedResponse();
            return ret;
        }

        ret.PagedResponse = await Repository.StreamGroup.GetPagedStreamGroups(request.Parameters).ConfigureAwait(false);
        return ret;


    }
}