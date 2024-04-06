namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SendSMWarnRequest(string Detail, string Summary = "Warning") : IRequest<APIResponse>;

internal class SendSMWarnHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    : IRequestHandler<SendSMWarnRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SendSMWarnRequest request, CancellationToken cancellationToken)
    {
        SMMessage sMMessage = new("warn", request.Summary, request.Detail);
        await hubContext.Clients.All.SendMessage(sMMessage).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
