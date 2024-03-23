namespace StreamMaster.Application.SMMessages.Commands;

[SMAPI]
public record SendSuccessRequest(string Detail, string Summary = "Success") : IRequest<DefaultAPIResponse>;

internal class SendSuccessHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
    : IRequestHandler<SendSuccessRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(SendSuccessRequest request, CancellationToken cancellationToken)
    {
        SMMessage sMMessage = new("success", request.Summary, request.Detail);
        await hubContext.Clients.All.SendMessage(sMMessage).ConfigureAwait(false);
        return APIResponseFactory.Ok;
    }
}
