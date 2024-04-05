namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMStreamVisibleByIdRequest(string Id) : IRequest<DefaultAPIResponse>;
internal class ToggleSMStreamVisibleByIdHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ToggleSMStreamVisibleByIdRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(ToggleSMStreamVisibleByIdRequest request, CancellationToken cancellationToken)
    {
        SMStreamDto? stream = await Repository.SMStream.ToggleSMStreamVisibleById(request.Id, cancellationToken).ConfigureAwait(false);
        if (stream == null)
        {
            return DefaultAPIResponse.NotFound;
        }

        FieldData fd = new(nameof(SMStreamDto), stream.Id, "isHidden", stream.IsHidden);

        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        return DefaultAPIResponse.Success;
    }
}
