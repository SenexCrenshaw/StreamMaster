namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelNumberRequest(int SMChannelId, int channelNumber) : IRequest<DefaultAPIResponse>;

internal class SetSMChannelChannelNumberRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    : IRequestHandler<SetSMChannelNumberRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(SetSMChannelNumberRequest request, CancellationToken cancellationToken)
    {
        DefaultAPIResponse ret = await Repository.SMChannel.SetSMChannelChannelNumber(request.SMChannelId, request.channelNumber).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set number failed {ret.Message}");
            return ret;
        }

        FieldData fd = new(nameof(SMChannelDto), request.SMChannelId.ToString(), "channelNumber", request.channelNumber);

        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        await messageService.SendSuccess($"Set number to '{request.channelNumber}'");
        return ret;
    }
}
