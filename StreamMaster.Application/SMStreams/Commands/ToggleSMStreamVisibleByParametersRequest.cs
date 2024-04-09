namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ToggleSMStreamVisibleByParametersRequest(QueryStringParameters Parameters) : IRequest<APIResponse>;
internal class ToggleSMStreamVisibleByParametersRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : IRequestHandler<ToggleSMStreamVisibleByParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ToggleSMStreamVisibleByParametersRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> ret = await Repository.SMStream.ToggleSMStreamVisibleByParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        if (ret.Count == 0)
        {
            return APIResponse.NotFound;
        }

        await hubContext.Clients.All.SetField(ret).ConfigureAwait(false);
        await hubContext.Clients.All.ClearByTag(new ClearByTag("GetPagedSMStreams", "IsHidden")).ConfigureAwait(false);
        return APIResponse.Success;
    }
}
