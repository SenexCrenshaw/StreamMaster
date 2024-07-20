namespace StreamMaster.Application.StreamGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetStreamGroupProfilesRequest() : IRequest<DataResponse<List<StreamGroupProfile>>>;

[LogExecutionTimeAspect]
internal class GetStreamGroupProfilesRequestHandler(IRepositoryWrapper Repository, IMapper mapper)
    : IRequestHandler<GetStreamGroupProfilesRequest, DataResponse<List<StreamGroupProfile>>>
{
    public async Task<DataResponse<List<StreamGroupProfile>>> Handle(GetStreamGroupProfilesRequest request, CancellationToken cancellationToken = default)
    {
        var streamGroups = Repository.StreamGroup.GetQuery().SelectMany(a => a.StreamGroupProfiles).OrderBy(a => a.ProfileName).ToList();

        return DataResponse<List<StreamGroupProfile>>.Success(streamGroups);
    }
}