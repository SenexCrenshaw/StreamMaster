namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelsVideoOutputProfileNameFromParametersRequest(QueryStringParameters Parameters, string VideoOutputProfileName) : IRequest<APIResponse>;

internal class SetSMChannelsVideoOutputProfileNameFromParametersRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelsVideoOutputProfileNameFromParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelsVideoOutputProfileNameFromParametersRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelsVideoOutputProfileNameFromParameters(request.Parameters, request.VideoOutputProfileName).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set Group failed {ret.Message}");
            return ret;
        }
        await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);

        return ret;
    }
}
