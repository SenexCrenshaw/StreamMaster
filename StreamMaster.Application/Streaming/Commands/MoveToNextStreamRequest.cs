namespace StreamMaster.Application.Streaming.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record MoveToNextStreamRequest(int SMChannelId) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class MoveToNextStreamRequestHandler(IChannelService ChannelService, IMessageService messageService)
    : IRequestHandler<MoveToNextStreamRequest, APIResponse>
{
    public async Task<APIResponse> Handle(MoveToNextStreamRequest request, CancellationToken cancellationToken)
    {
        if (request.SMChannelId < 1)
        {
            await messageService.SendWarning("SMChannelId < 1");
            return APIResponse.NotFound;
        }
        await ChannelService.MoveToNextStreamAsync(request.SMChannelId);

        await messageService.SendSuccess("Successful", "Move To Next Stream");
        return APIResponse.Ok;
    }
}
