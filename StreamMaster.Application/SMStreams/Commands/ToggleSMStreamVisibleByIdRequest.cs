namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMStreamVisibleByIdRequest(string Id) : IRequest<APIResponse>;
internal class ToggleSMStreamVisibleByIdHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<ToggleSMStreamVisibleByIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMStreamVisibleByIdRequest request, CancellationToken cancellationToken)
    {
        SMStreamDto? stream = await Repository.SMStream.ToggleSMStreamVisibleById(request.Id, cancellationToken).ConfigureAwait(false);
        if (stream == null)
        {
            return APIResponse.NotFound;
        }

        FieldData fd = new(SMStream.APIName, stream.Id, "IsHidden", stream.IsHidden);

        await dataRefreshService.SetField([fd]).ConfigureAwait(false);
        await dataRefreshService.ClearByTag(SMStream.APIName, "IsHidden").ConfigureAwait(false);
        return APIResponse.Success;
    }
}
