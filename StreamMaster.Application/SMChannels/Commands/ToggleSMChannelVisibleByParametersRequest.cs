namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMChannelVisibleByParametersRequest(QueryStringParameters Parameters) : IRequest<APIResponse>;
internal class ToggleSMChannelVisibleByParametersRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<ToggleSMChannelVisibleByParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMChannelVisibleByParametersRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> ret = await Repository.SMChannel.ToggleSMChannelVisibleByParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        if (ret.Count == 0)
        {
            return APIResponse.NotFound;
        }

        await dataRefreshService.SetField(ret).ConfigureAwait(false);
        await dataRefreshService.ClearByTag(SMChannel.APIName, "IsHidden").ConfigureAwait(false);

        return APIResponse.Success;
    }
}
