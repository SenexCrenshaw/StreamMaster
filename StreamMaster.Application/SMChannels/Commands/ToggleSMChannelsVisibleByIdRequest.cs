namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMChannelsVisibleByIdRequest(List<int> Ids) : IRequest<APIResponse>;
internal class ToggleSMChannelsVisibleByIdHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ToggleSMChannelsVisibleByIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMChannelsVisibleByIdRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> ret = await Repository.SMChannel.ToggleSMChannelsVisibleById(request.Ids, cancellationToken).ConfigureAwait(false);
        if (ret.Count == 0)
        {
            return APIResponse.NotFound;
        }

        await hubContext.Clients.All.SetField(ret).ConfigureAwait(false);
        await hubContext.Clients.All.ClearByTag(new ClearByTag("GetPagedSMChannels", "IsHidden")).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
