namespace StreamMaster.Application.SMStreams.Commands;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ClearByTag(string Entity, string Tag) { }


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMStreamsVisibleByIdRequest(List<string> Ids) : IRequest<APIResponse>;
internal class ToggleSMStreamsVisibleByIdHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ToggleSMStreamsVisibleByIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMStreamsVisibleByIdRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> ret = await Repository.SMStream.ToggleSMStreamsVisibleById(request.Ids, cancellationToken).ConfigureAwait(false);
        if (ret.Count == 0)
        {
            return APIResponse.NotFound;
        }

        await hubContext.Clients.All.SetField(ret).ConfigureAwait(false);
        await hubContext.Clients.All.ClearByTag(new ClearByTag("GetPagedSMStreams", "IsHidden")).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
