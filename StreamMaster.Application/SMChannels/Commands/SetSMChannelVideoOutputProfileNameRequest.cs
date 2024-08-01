namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelCommandProfileNameRequest(int SMChannelId, string CommandProfileName) : IRequest<APIResponse>;

internal class SetSMChannelCommandProfileNameRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelCommandProfileNameRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelCommandProfileNameRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelCommandProfileName(request.SMChannelId, request.CommandProfileName).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set EPG failed {ret.Message}");
            return ret;
        }

        await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);

        return ret;
        //return APIResponse.Success;
    }
}
