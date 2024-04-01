namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelNameRequest(int SMChannelId, string name) : IRequest<DefaultAPIResponse>;

internal class SetSMChannelNameRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<SetSMChannelNameRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(SetSMChannelNameRequest request, CancellationToken cancellationToken)
    {
        DefaultAPIResponse ret = await Repository.SMChannel.SetSMChannelName(request.SMChannelId, request.name).ConfigureAwait(false);
        if (ret.IsError.HasValue && ret.IsError.Value)
        {
            await messageService.SendError($"Set name failed {ret.Message}");
            return ret;
        }

        FieldData fd = new(nameof(SMChannelDto), request.SMChannelId.ToString(), "name", request.name);

        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        await messageService.SendSuccess($"Set name to '{request.name}'");
        return ret;
    }
}
