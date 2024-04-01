namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelLogoRequest(int SMChannelId, string logo) : IRequest<DefaultAPIResponse>;

internal class SetSMChannelLogoRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<SetSMChannelLogoRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(SetSMChannelLogoRequest request, CancellationToken cancellationToken)
    {
        DefaultAPIResponse ret = await Repository.SMChannel.SetSMChannelLogo(request.SMChannelId, request.logo).ConfigureAwait(false);
        if (ret.IsError.HasValue && ret.IsError.Value)
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
