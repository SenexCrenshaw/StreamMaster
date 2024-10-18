namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelsCommandProfileNameFromParametersRequest(QueryStringParameters Parameters, string CommandProfileName) : IRequest<APIResponse>;

internal class SetSMChannelsCommandProfileNameFromParametersRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelsCommandProfileNameFromParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelsCommandProfileNameFromParametersRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelsCommandProfileNameFromParameters(request.Parameters, request.CommandProfileName).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set Group failed {ret.Message}");
            return ret;
        }
        await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);

        return ret;
        //return APIResponse.Success;
    }
}
