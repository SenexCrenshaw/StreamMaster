namespace StreamMaster.Application.SMChannels.Commands;
[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AutoSetEPGRequest(List<int> Ids) : IRequest<APIResponse>;

public class AutoSetEPGRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<AutoSetEPGRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AutoSetEPGRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> results = await Repository.SMChannel.AutoSetEPGFromIds(request.Ids, cancellationToken).ConfigureAwait(false);
        if (results.Count > 0)
        {
            //await dataRefreshService.RefreshSMChannels().ConfigureAwait(false);
            await dataRefreshService.SetField(results);
            await messageService.SendSuccess("Auto Set EPG For Channels");
        }
        return APIResponse.Ok;
    }
}
