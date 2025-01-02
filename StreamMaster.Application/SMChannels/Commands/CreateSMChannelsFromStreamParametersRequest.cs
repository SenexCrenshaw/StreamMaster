namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateSMChannelsFromStreamParametersRequest(QueryStringParameters Parameters, string? DefaultStreamGroupName, int M3UFileId, int? StreamGroupId) : IRequest<APIResponse>;

internal class CreateSMChannelsFromStreamParametersRequestHandler(IRepositoryWrapper Repository,ISMWebSocketManager sMWebSocketManager, IDataRefreshService dataRefreshService)
    : IRequestHandler<CreateSMChannelsFromStreamParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateSMChannelsFromStreamParametersRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.SMChannel.CreateSMChannelsFromStreamParameters(request.Parameters, request.StreamGroupId);
        if (res.IsError)
        {
            return APIResponse.ErrorWithMessage(res.ErrorMessage);
        }

        await dataRefreshService.RefreshAllSMChannels();
        await sMWebSocketManager.BroadcastReloadAsync();
        return APIResponse.Success;
    }
}
