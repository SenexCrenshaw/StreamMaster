﻿namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
public record SendSMWarnRequest(string Detail, string Summary = "Warning") : IRequest<DefaultAPIResponse>;

internal class SendSMWarnHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    : IRequestHandler<SendSMWarnRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(SendSMWarnRequest request, CancellationToken cancellationToken)
    {
        SMMessage sMMessage = new("warn", request.Summary, request.Detail);
        await hubContext.Clients.All.SendMessage(sMMessage).ConfigureAwait(false);
        return APIResponseFactory.Ok;
    }
}