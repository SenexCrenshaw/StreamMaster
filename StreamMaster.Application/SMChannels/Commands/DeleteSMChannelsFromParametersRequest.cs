namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteSMChannelsFromParametersRequest(QueryStringParameters Parameters) : IRequest<APIResponse>;

internal class DeleteSMChannelsFromParametersRequestHandler(IRepositoryWrapper Repository, ISMWebSocketManager sMWebSocketManager, IDataRefreshService dataRefreshService) : IRequestHandler<DeleteSMChannelsFromParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteSMChannelsFromParametersRequest request, CancellationToken cancellationToken)
    {
        await Repository.SMChannel.DeleteSMChannelsFromParameters(request.Parameters).ConfigureAwait(false);


        await dataRefreshService.RefreshAllSMChannels();
        await sMWebSocketManager.BroadcastReloadAsync();


        return APIResponse.Success;
    }
}
