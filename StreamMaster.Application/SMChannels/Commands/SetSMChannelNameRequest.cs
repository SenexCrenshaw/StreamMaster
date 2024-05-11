namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelNameRequest(int SMChannelId, string Name) : IRequest<APIResponse>;

internal class SetSMChannelNameRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelNameRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelNameRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelName(request.SMChannelId, request.Name).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set name failed {ret.Message}");
            return ret;
        }

        FieldData fd = new(SMChannel.MainGet, request.SMChannelId, "Name", request.Name);

        await dataRefreshService.SetField([fd]).ConfigureAwait(false);
        //await messageService.SendSuccess($"Set name to '{request.Name}'");
        return ret;
    }
}
