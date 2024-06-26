namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SendSMInfoRequest(string Detail, string Summary = "Info") : IRequest<APIResponse>;

internal class SendSMInfoHandler(IDataRefreshService dataRefreshService) : IRequestHandler<SendSMInfoRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SendSMInfoRequest request, CancellationToken cancellationToken)
    {
        SMMessage sMMessage = new("info", request.Summary, request.Detail);
        await dataRefreshService.SendMessage(sMMessage).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
