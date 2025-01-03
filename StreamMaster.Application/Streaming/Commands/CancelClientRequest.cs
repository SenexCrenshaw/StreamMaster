namespace StreamMaster.Application.Streaming.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CancelClientRequest(string UniqueRequestId) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CancelClientStreamerRequestHandler(IChannelService ChannelService, IMessageService messageService)
    : IRequestHandler<CancelClientRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CancelClientRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.UniqueRequestId))
        {
            await messageService.SendWarning("Client Cancelled Failed");
            return APIResponse.NotFound;
        }
        await ChannelService.UnRegisterClientAsync(request.UniqueRequestId, cancellationToken: cancellationToken);

        await messageService.SendSuccess("Client Cancelled Successfully", "Client Cancel");
        return APIResponse.Ok;
    }
}
