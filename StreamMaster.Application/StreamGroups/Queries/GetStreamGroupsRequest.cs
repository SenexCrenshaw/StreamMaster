namespace StreamMaster.Application.StreamGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamGroupsRequest() : IRequest<DataResponse<List<StreamGroupDto>>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupsRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetStreamGroupsRequest, DataResponse<List<StreamGroupDto>>>
{
    public async Task<DataResponse<List<StreamGroupDto>>> Handle(GetStreamGroupsRequest request, CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto> streamGroups = await Repository.StreamGroup.GetStreamGroups(cancellationToken);
        foreach (StreamGroupDto streamGroupDto in streamGroups)
        {
            streamGroupDto.ChannelCount = Repository.StreamGroupSMChannelLink.GetQuery().Count(a => a.StreamGroupId == streamGroupDto.Id);
        }
        return DataResponse<List<StreamGroupDto>>.Success(streamGroups);
    }
}