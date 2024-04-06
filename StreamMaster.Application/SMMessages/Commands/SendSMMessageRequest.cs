namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SendSMMessageRequest(SMMessage Message) : IRequest<APIResponse>;

internal class SendSMMessageHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<SendSMMessageRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SendSMMessageRequest request, CancellationToken cancellationToken)
    {
        await hubContext.Clients.All.SendMessage(request.Message).ConfigureAwait(false);

        return APIResponse.Success;
    }
}
