namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelVideoOutputProfileNameRequest(int SMChannelId, string VideoOutputProfileName) : IRequest<APIResponse>;

internal class SetSMChannelVideoOutputProfileNameRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelVideoOutputProfileNameRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelVideoOutputProfileNameRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelVideoOutputProfileName(request.SMChannelId, request.VideoOutputProfileName).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set EPG failed {ret.Message}");
            return ret;
        }

        await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);

        return ret;
    }
}
