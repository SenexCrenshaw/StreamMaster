namespace StreamMaster.Application.Streaming.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CancelClientRequest(string UniqueRequestId) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CancelClientStreamerRequestHandler(IChannelManager ChannelManager, IMessageService messageService)
    : IRequestHandler<CancelClientRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CancelClientRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UniqueRequestId))
        {
            await messageService.SendWarn("Client Cancelled Failed");
            return APIResponse.NotFound;
        }
        await ChannelManager.CancelClient(request.UniqueRequestId);

        await messageService.SendSuccess("Client Cancelled Successfully", "Client Cancel");
        return APIResponse.Ok;
    }
}
