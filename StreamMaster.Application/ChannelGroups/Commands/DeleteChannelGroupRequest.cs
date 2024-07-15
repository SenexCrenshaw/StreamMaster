namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteChannelGroupRequest(int ChannelGroupId) : IRequest<APIResponse>;

public class DeleteChannelGroupRequestRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<DeleteChannelGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteChannelGroupRequest request, CancellationToken cancellationToken)
    {
        APIResponse result = await Repository.ChannelGroup.DeleteChannelGroupsRequest([request.ChannelGroupId]).ConfigureAwait(false);
        await dataRefreshService.RefreshChannelGroups().ConfigureAwait(false);
        return result;
    }
}
