namespace StreamMaster.Application.StreamGroups.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetDefaultStreamGroupProfileIdRequest() : IRequest<DataResponse<StreamGroupProfile>>;

[LogExecutionTimeAspect]
internal class GetDefaultStreamGroupProfileIdRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetDefaultStreamGroupProfileIdRequest, DataResponse<StreamGroupProfile>>
{
    public async Task<DataResponse<StreamGroupProfile>> Handle(GetDefaultStreamGroupProfileIdRequest request, CancellationToken cancellationToken = default)
    {
        StreamGroup? allStreamgroup = await Repository.StreamGroup.GetQuery().FirstOrDefaultAsync(a => a.Name.ToLower() == "all");
        if (allStreamgroup == null)
        {
            return DataResponse<StreamGroupProfile>.NotFound;
        }

        StreamGroupProfile defaultPolicy = await Repository.StreamGroupProfile.GetQuery().FirstAsync(a => a.StreamGroupId == allStreamgroup.Id && a.ProfileName.ToLower() == "default");
        return DataResponse<StreamGroupProfile>.Success(defaultPolicy);
    }
}