namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
public record ToggleSMStreamVisibleById(string Id) : IRequest<DefaultAPIResponse>;

internal class ToggleSMStreamVisibleByIdHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ToggleSMStreamVisibleById, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(ToggleSMStreamVisibleById request, CancellationToken cancellationToken)
    {
        SMStreamDto? stream = await Repository.SMStream.ToggleSMStreamVisibleById(request.Id, cancellationToken).ConfigureAwait(false);
        if (stream == null)
        {
            return APIResponseFactory.NotFound;
        }

        FieldData fd = new(nameof(SMStreamDto), stream.Id, "isHidden", stream.IsHidden);

        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        return APIResponseFactory.Ok;
    }
}
