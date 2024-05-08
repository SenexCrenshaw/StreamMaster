namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMStreamVisibleByIdRequest(string Id) : IRequest<APIResponse>;
internal class ToggleSMStreamVisibleByIdHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ToggleSMStreamVisibleByIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMStreamVisibleByIdRequest request, CancellationToken cancellationToken)
    {
        SMStreamDto? stream = await Repository.SMStream.ToggleSMStreamVisibleById(request.Id, cancellationToken).ConfigureAwait(false);
        if (stream == null)
        {
            return APIResponse.NotFound;
        }

        FieldData fd = new(SMStream.MainGet, stream.Id, "IsHidden", stream.IsHidden);

        await hubContext.Clients.All.SetField([fd]).ConfigureAwait(false);
        await hubContext.Clients.All.ClearByTag(new ClearByTag("GetPagedSMStreams", "IsHidden")).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
