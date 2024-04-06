namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelLogoRequest(int SMChannelId, string logo) : IRequest<APIResponse>;

internal class SetSMChannelLogoRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<SetSMChannelLogoRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelLogoRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelLogo(request.SMChannelId, request.logo).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set logo failed {ret.Message}");
            return ret;
        }

        FieldData fd = new(nameof(SMChannelDto), request.SMChannelId.ToString(), "logo", request.logo);
        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        await messageService.SendSuccess($"Set logo {ret.Message}");
        return ret;
    }
}
