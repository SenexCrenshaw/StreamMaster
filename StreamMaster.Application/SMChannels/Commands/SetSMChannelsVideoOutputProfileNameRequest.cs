namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelsCommandProfileNameRequest(List<int> SMChannelIds, string CommandProfileName) : IRequest<APIResponse>;

internal class SetSMChannelsCommandProfileNameRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelsCommandProfileNameRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelsCommandProfileNameRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelsCommandProfileName(request.SMChannelIds, request.CommandProfileName).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set CommandProfileName failed {ret.Message}");
            return ret;
        }
        await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);

        return ret;
        //return APIResponse.Success;
    }
}

