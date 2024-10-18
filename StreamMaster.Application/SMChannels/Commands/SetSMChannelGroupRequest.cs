namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelGroupRequest(int SMChannelId, string Group) : IRequest<APIResponse>;

internal class SetSMChannelGroupRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelGroupRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelGroup(request.SMChannelId, request.Group).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set Group failed {ret.Message}");
            return ret;
        }

        //FieldData fd = new(SMChannel.APIName, request.Id, "CommandProfileName", request.CommandProfileName);
        //await dataRefreshService.SetField([fd]).ConfigureAwait(false);
        await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);

        return ret;
    }
}
