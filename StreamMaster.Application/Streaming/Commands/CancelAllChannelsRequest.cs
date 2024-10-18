namespace StreamMaster.Application.Streaming.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CancelAllChannelsRequest() : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CancelAllChannelsRequestHandler(IChannelManager ChannelManager, IMessageService messageService)
    : IRequestHandler<CancelAllChannelsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CancelAllChannelsRequest request, CancellationToken cancellationToken)
    {
        ChannelManager.CancelAllChannels();
        await messageService.SendSuccess("All Channels Cancelled Successfully", "Channel Cancel");
        return APIResponse.Ok;
    }
}
