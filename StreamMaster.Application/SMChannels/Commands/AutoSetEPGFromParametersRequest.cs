namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AutoSetEPGFromParametersRequest(QueryStringParameters Parameters) : IRequest<APIResponse> { }

[LogExecutionTimeAspect]
public class AutoSetEPGFromParametersRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<AutoSetEPGFromParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AutoSetEPGFromParametersRequest request, CancellationToken cancellationToken)
    {
        var results = await Repository.SMChannel.AutoSetEPGFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
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
