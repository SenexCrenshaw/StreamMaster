namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMChannelsVisibleByIdRequest(List<int> Ids) : IRequest<APIResponse>;
internal class ToggleSMChannelsVisibleByIdHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<ToggleSMChannelsVisibleByIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMChannelsVisibleByIdRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> ret = await Repository.SMChannel.ToggleSMChannelsVisibleById(request.Ids, cancellationToken).ConfigureAwait(false);
        if (ret.Count == 0)
        {
            return APIResponse.NotFound;
        }

        await dataRefreshService.SetField(ret).ConfigureAwait(false);
        await dataRefreshService.ClearByTag(SMChannel.APIName, "IsHidden").ConfigureAwait(false);
        return APIResponse.Success;
    }
}
