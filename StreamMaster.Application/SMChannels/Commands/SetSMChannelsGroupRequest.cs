namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelsGroupRequest(List<int> SMChannelIds, string Group) : IRequest<APIResponse>;

internal class SetSMChannelsGroupRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelsGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelsGroupRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelsGroup(request.SMChannelIds, request.Group).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set Group failed {ret.Message}");
            return ret;
        }
        await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);

        return ret;
    }
}

