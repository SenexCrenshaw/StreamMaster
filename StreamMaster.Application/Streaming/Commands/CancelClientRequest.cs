namespace StreamMaster.Application.Streaming.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CancelClientRequest(Guid ClientId) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CancelClientStreamerRequestHandler(IChannelManager ChannelManager, IMessageService messageService)
    : IRequestHandler<CancelClientRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CancelClientRequest request, CancellationToken cancellationToken)
    {
        if (request.ClientId == Guid.Empty)
        {
            await messageService.SendWarn("Client Cancelled Failed");
            return APIResponse.NotFound;
        }
        ChannelManager.CancelClient(request.ClientId);

        await messageService.SendSuccess("Client Cancelled Successfully", "Client Cancel");
        return APIResponse.Ok;
    }
}
