namespace StreamMaster.Application.Streaming.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CancelChannelRequest(int SMChannelId) : IRequest<APIResponse> { }

[LogExecutionTimeAspect]
public class CancelChannelRequestHandler(IChannelManager ChannelManager, IMessageService messageService)
    : IRequestHandler<CancelChannelRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CancelChannelRequest request, CancellationToken cancellationToken)
    {
        if (request.SMChannelId < 1)
        {
            await messageService.SendWarn("Channel Cancelled failed");
            return APIResponse.NotFound;
        }
        await ChannelManager.CancelChannel(request.SMChannelId);

        await messageService.SendSuccess("Channel Cancelled Successfully", "Channel Cancel");
        return APIResponse.Ok;
    }
}
