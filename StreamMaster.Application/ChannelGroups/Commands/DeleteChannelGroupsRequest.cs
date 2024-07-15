namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteChannelGroupsRequest(List<int> ChannelGroupIds) : IRequest<APIResponse>;

public class DeleteChannelGroupRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<DeleteChannelGroupsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteChannelGroupsRequest request, CancellationToken cancellationToken)
    {
        APIResponse result = await Repository.ChannelGroup.DeleteChannelGroupsRequest(request.ChannelGroupIds).ConfigureAwait(false);
        await dataRefreshService.RefreshChannelGroups().ConfigureAwait(false);
        return result;
    }
}
