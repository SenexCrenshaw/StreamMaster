namespace StreamMaster.Application.ChannelGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteAllChannelGroupsFromParametersRequest(QueryStringParameters Parameters) : IRequest<APIResponse> { }

public class DeleteAllChannelGroupsFromParametersRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService)
    : IRequestHandler<DeleteAllChannelGroupsFromParametersRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteAllChannelGroupsFromParametersRequest request, CancellationToken cancellationToken)
    {
        APIResponse res = await Repository.ChannelGroup.DeleteAllChannelGroupsFromParameters(request.Parameters, cancellationToken).ConfigureAwait(false);
        //await Repository.VideoStream.UpdateVideoStreamsChannelGroupNames(VideoStreams.Select(a => a.Id), "").ConfigureAwait(false);
        _ = await Repository.SaveAsync().ConfigureAwait(false);
        await dataRefreshService.RefreshChannelGroups().ConfigureAwait(false);
        return res;
    }
}
