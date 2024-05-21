namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelLogoRequest(int SMChannelId, string Logo) : IRequest<APIResponse>;

internal class SetSMChannelLogoRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<SetSMChannelLogoRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelLogoRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelLogo(request.SMChannelId, request.Logo).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set logo failed {ret.Message}");
            return ret;
        }

        FieldData fd = new(SMChannel.APIName, request.SMChannelId, "Logo", request.Logo);
        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        await hubContext.Clients.All.DataRefresh("GetSMChannel");
        return ret;
    }
}
