namespace StreamMaster.Application.StreamGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedStreamGroupsRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetPagedStreamGroupsRequestHandler(IRepositoryWrapper Repository, IStreamGroupService streamGroupService)
    : IRequestHandler<GetPagedStreamGroupsRequest, PagedResponse<StreamGroupDto>>
{
    public async Task<PagedResponse<StreamGroupDto>> Handle(GetPagedStreamGroupsRequest request, CancellationToken cancellationToken = default)
    {
        PagedResponse<StreamGroupDto> res = request.Parameters.PageSize == 0
            ? Repository.StreamGroup.CreateEmptyPagedResponse()
            : await Repository.StreamGroup.GetPagedStreamGroups(request.Parameters).ConfigureAwait(false);

        int defaultSGId = await streamGroupService.GetDefaultSGIdAsync();

        if (res.Data is null)
        {
            return res;
        }

        foreach (StreamGroupDto streamGroupDto in res.Data)
        {
            if (streamGroupDto.Id == defaultSGId)
            {
                streamGroupDto.ChannelCount = Repository.SMChannel.Count();
                continue;
            }

            streamGroupDto.ChannelCount = Repository.StreamGroupSMChannelLink.GetQuery().Count(a => a.StreamGroupId == streamGroupDto.Id);
        }

        return res;
    }
}