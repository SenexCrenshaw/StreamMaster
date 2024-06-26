namespace StreamMaster.Application.StreamGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamGroupRequest(string SGName) : IRequest<DataResponse<StreamGroupDto>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetStreamGroupRequest, DataResponse<StreamGroupDto>>
{
    public async Task<DataResponse<StreamGroupDto>> Handle(GetStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        StreamGroup? streamGroup = await Repository.StreamGroup.FirstOrDefaultAsync(a => a.Name.Equals(request.SGName));
        if (streamGroup == null)
        {
            return DataResponse<StreamGroupDto>.ErrorWithMessage("Stream Group not found");
        }

        StreamGroupDto streamGroupDto = mapper.Map<StreamGroupDto>(streamGroup);

        streamGroupDto.ChannelCount = Repository.StreamGroupSMChannelLink.GetQuery().Count(a => a.StreamGroupId == streamGroup.Id);

        return DataResponse<StreamGroupDto>.Success(streamGroupDto);
    }
}