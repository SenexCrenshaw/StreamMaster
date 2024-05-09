namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMChannelVisibleByParametersRequest(QueryStringParameters Parameters) : IRequest<APIResponse>;
internal class ToggleSMChannelVisibleByParametersRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ToggleSMChannelVisibleByParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMChannelVisibleByParametersRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> ret = await Repository.SMChannel.ToggleSMChannelVisibleByParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        if (ret.Count == 0)
        {
            return APIResponse.NotFound;
        }

        await hubContext.Clients.All.SetField(ret).ConfigureAwait(false);
        await hubContext.Clients.All.ClearByTag(new ClearByTag("GetPagedSMChannels", "IsHidden")).ConfigureAwait(false);

        return APIResponse.Success;
    }
}
