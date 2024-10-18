namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SetSMChannelsLogoFromEPGFromParametersRequest(QueryStringParameters Parameters) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class SetSMChannelsLogoFromEPGFromParametersRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<SetSMChannelsLogoFromEPGFromParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(SetSMChannelsLogoFromEPGFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<FieldData> results = await Repository.SMChannel.SetSMChannelsLogoFromEPGFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        if (results.Count != 0)
        {
            await dataRefreshService.SetField(results);
            await messageService.SendSuccess($"Set Logo From EPG For Channels");
        }
        return APIResponse.Ok;
    }
}