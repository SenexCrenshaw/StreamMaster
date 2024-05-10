namespace StreamMaster.Application.SMChannels.Commands;
[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AutoSetEPGRequest(List<int> Ids) : IRequest<APIResponse> { }


public class AutoSetEPGRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<AutoSetEPGRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AutoSetEPGRequest request, CancellationToken cancellationToken)
    {
        var results = await Repository.SMChannel.AutoSetEPGFromIds(request.Ids, cancellationToken).ConfigureAwait(false);

        if (results.Any())
        {
            var fds = new List<FieldData>();
            foreach (var result in results)
            {
                fds.Add(new FieldData(SMChannel.MainGet, result.Id, "Logo", result.Logo));
                fds.Add(new FieldData(SMChannel.MainGet, result.Id, "EPGId", result.EPGId));
            }
            //await dataRefreshService.RefreshAllSMChannels();
            await dataRefreshService.SetField(fds);
            await messageService.SendSuccess($"Auto Set EPG For Channels");
        }
        return APIResponse.Ok;
    }
}
