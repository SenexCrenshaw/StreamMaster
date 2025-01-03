namespace StreamMaster.Application.WebSocket.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record TriggerReloadRequest() : IRequest<APIResponse>;


public class TriggerReloadRequestHandler(ISMWebSocketManager smWebSocketManager)
    : IRequestHandler<TriggerReloadRequest, APIResponse>
{
    public async Task<APIResponse> Handle(TriggerReloadRequest request, CancellationToken cancellationToken)
    {
        await smWebSocketManager.BroadcastReloadAsync();
        return APIResponse.Ok;
    }
}
