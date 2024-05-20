namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMStreamVisibleByParametersRequest(QueryStringParameters Parameters) : IRequest<APIResponse>;
internal class ToggleSMStreamVisibleByParametersRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<ToggleSMStreamVisibleByParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMStreamVisibleByParametersRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> ret = await Repository.SMStream.ToggleSMStreamVisibleByParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        if (ret.Count == 0)
        {
            return APIResponse.NotFound;
        }

        await dataRefreshService.SetField(ret).ConfigureAwait(false);
        await dataRefreshService.ClearByTag(SMStream.MainGet, "IsHidden").ConfigureAwait(false);
        return APIResponse.Success;
    }
}
