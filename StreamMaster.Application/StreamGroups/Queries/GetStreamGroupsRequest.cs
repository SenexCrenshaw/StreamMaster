using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

namespace StreamMaster.Application.StreamGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamGroupsRequest() : IRequest<DataResponse<List<StreamGroupDto>>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupsRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetStreamGroupsRequest, DataResponse<List<StreamGroupDto>>>
{
    public async Task<DataResponse<List<StreamGroupDto>>> Handle(GetStreamGroupsRequest request, CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto> streamGroups = await Repository.StreamGroup.GetQuery().OrderBy(a => a.Name).ProjectTo<StreamGroupDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken);
        return DataResponse<List<StreamGroupDto>>.Success(streamGroups);
    }
}