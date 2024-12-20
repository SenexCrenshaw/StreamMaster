namespace StreamMaster.Application.Streaming.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CancelAllChannelsRequest() : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class CancelAllChannelsRequestHandler(IChannelService ChannelService, IMessageService messageService)
    : IRequestHandler<CancelAllChannelsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CancelAllChannelsRequest request, CancellationToken cancellationToken)
    {
        await ChannelService.CancelAllChannelsAsync();
        await messageService.SendSuccess("All Channels Cancelled Successfully", "Channel Cancel");
        return APIResponse.Ok;
    }
}
