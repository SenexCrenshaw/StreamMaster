namespace StreamMaster.Application.SMChannels.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteSMChannelsRequest(List<int> SMChannelIds) : IRequest<APIResponse>;

internal class DeleteSMChannelsRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService) : IRequestHandler<DeleteSMChannelsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteSMChannelsRequest request, CancellationToken cancellationToken)
    {
        APIResponse ret = await Repository.SMChannel.DeleteSMChannels(request.SMChannelIds);
        if (!ret.IsError)
        {
            await dataRefreshService.RefreshAllSMChannels();
        }

        return ret;
    }
}
