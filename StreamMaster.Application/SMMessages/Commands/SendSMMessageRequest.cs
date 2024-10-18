namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SendSMMessageRequest(SMMessage Message) : IRequest<APIResponse>;

internal class SendSMMessageHandler(IDataRefreshService dataRefreshService) : IRequestHandler<SendSMMessageRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SendSMMessageRequest request, CancellationToken cancellationToken)
    {
        await dataRefreshService.SendMessage(request.Message).ConfigureAwait(false);

        return APIResponse.Success;
    }
}
