﻿namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SendSMErrorRequest(string Detail, string Summary = "Error") : IRequest<APIResponse>;

internal class SendSMErrorRequestHandler(IDataRefreshService dataRefreshService)
    : IRequestHandler<SendSMErrorRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SendSMErrorRequest request, CancellationToken cancellationToken)
    {
        SMMessage sMMessage = new("error", request.Summary, request.Detail);
        await dataRefreshService.SendMessage(sMMessage).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
