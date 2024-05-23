namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelsLogoFromEPGRequest(List<int> Ids) : IRequest<APIResponse> { }

[LogExecutionTimeAspect]
public class SetSMChannelsLogoFromEPGRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<SetSMChannelsLogoFromEPGRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelsLogoFromEPGRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> results = await Repository.SMChannel.SetSMChannelsLogoFromEPGFromIds(request.Ids, cancellationToken).ConfigureAwait(false);
        if (results.Count != 0)
        {
            await dataRefreshService.SetField(results);
            await messageService.SendSuccess($"Set Logo From EPG For Channels");
        }
        return APIResponse.Ok;
    }
}
