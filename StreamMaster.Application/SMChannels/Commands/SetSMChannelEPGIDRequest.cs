namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelEPGIdRequest(int SMChannelId, string EPGId) : IRequest<APIResponse>;

internal class SetSMChannelEPGIdRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<SetSMChannelEPGIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelEPGIdRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelEPGID(request.SMChannelId, request.EPGId).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set EPG failed {ret.Message}");
            return ret;
        }

        FieldData fd = new(SMChannel.MainGet, request.SMChannelId, "EPGId", request.EPGId);
        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        //await messageService.SendSuccess($"Set EPG {ret.Message}");
        return ret;
    }
}
