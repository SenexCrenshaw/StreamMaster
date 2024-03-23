namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
public record SendSMMessageRequest(SMMessage Message) : IRequest<DefaultAPIResponse>;

internal class SendSMMessageHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<SendSMMessageRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(SendSMMessageRequest request, CancellationToken cancellationToken)
    {
        await hubContext.Clients.All.SendMessage(request.Message).ConfigureAwait(false);

        return APIResponseFactory.Ok;
    }
}
