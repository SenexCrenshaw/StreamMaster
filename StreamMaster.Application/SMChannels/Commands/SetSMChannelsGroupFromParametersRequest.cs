namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelsGroupFromParametersRequest(QueryStringParameters Parameters, string Group) : IRequest<APIResponse>;

internal class SetSMChannelsGroupFromParametersRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMChannelsGroupFromParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelsGroupFromParametersRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.SetSMChannelsGroupFromParameters(request.Parameters, request.Group).ConfigureAwait(false);
        if (ret.IsError)
        {
            await messageService.SendError($"Set Group failed {ret.Message}");
            return ret;
        }
        await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);

        return ret;
    }
}
