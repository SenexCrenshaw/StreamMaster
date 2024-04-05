namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SendSMErrorRequest(string Detail, string Summary = "Error") : IRequest<DefaultAPIResponse>;

internal class SendSMErrorHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    : IRequestHandler<SendSMErrorRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(SendSMErrorRequest request, CancellationToken cancellationToken)
    {
        SMMessage sMMessage = new("error", request.Summary, request.Detail);
        await hubContext.Clients.All.SendMessage(sMMessage).ConfigureAwait(false);
        return DefaultAPIResponse.Success;
    }
}
