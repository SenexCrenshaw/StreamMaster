namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteSMChannelsFromParametersRequest(QueryStringParameters Parameters) : IRequest<APIResponse>;

internal class DeleteSMChannelsFromParametersRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<DeleteSMChannelsFromParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteSMChannelsFromParametersRequest request, CancellationToken cancellationToken)
    {
        List<int> ids = await Repository.SMChannel.DeleteSMChannelsFromParameters(request.Parameters).ConfigureAwait(false);

        if (ids.Count != 0)
        {
            await dataRefreshService.RefreshAllSMChannels();
        }

        return APIResponse.Success;
    }
}
