namespace StreamMaster.Application.SMStreams.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMStreamsVisibleByIdRequest(List<string> Ids, bool IsHidden) : IRequest<APIResponse>;
internal class SetSMStreamsVisibleByIdRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<SetSMStreamsVisibleByIdRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMStreamsVisibleByIdRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> ret = await Repository.SMStream.SetSMStreamsVisibleById(request.Ids, request.IsHidden, cancellationToken).ConfigureAwait(false);
        if (ret.Count == 0)
        {
            return APIResponse.NotFound;
        }

        await dataRefreshService.SetField(ret).ConfigureAwait(false);
        await dataRefreshService.ClearByTag("GetPagedSMStreams", "IsHidden").ConfigureAwait(false);
        return APIResponse.Success;
    }
}
