namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SendSuccessRequest(string Detail, string Summary = "Success") : IRequest<APIResponse>;

internal class SendSuccessHandler(IDataRefreshService dataRefreshService)
    : IRequestHandler<SendSuccessRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SendSuccessRequest request, CancellationToken cancellationToken)
    {
        SMMessage sMMessage = new("success", request.Summary, request.Detail);
        await dataRefreshService.SendMessage(sMMessage).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
