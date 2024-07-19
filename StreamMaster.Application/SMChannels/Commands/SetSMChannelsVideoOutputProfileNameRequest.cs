namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelsVideoOutputProfileNameRequest(List<int> SMChannelIds, string VideoOutputProfileName) : IRequest<APIResponse>;

internal class SetSMChannelsVideoOutputProfileNameRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelsVideoOutputProfileNameRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelsVideoOutputProfileNameRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelsVideoOutputProfileName(request.SMChannelIds, request.VideoOutputProfileName).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set VideoOutputProfileName failed {ret.Message}");
            return ret;
        }
        await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);

        return ret;
    }
}

