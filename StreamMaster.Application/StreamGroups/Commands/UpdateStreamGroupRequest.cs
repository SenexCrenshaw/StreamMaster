using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateStreamGroupRequest(int StreamGroupId, string? NewName, bool? AutoSetChannelNumbers, List<string>? StreamGroupProfiles)
 : IRequest<APIResponse>
{ }


[LogExecutionTimeAspect]
public class UpdateStreamGroupRequestHandler(IDataRefreshService dataRefreshService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<UpdateStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || (string.IsNullOrEmpty(request.NewName) && string.IsNullOrEmpty(request.NewName)))
        {
            return APIResponse.NotFound;
        }

        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroup(request.StreamGroupId, request.NewName, request.AutoSetChannelNumbers);
        if (streamGroup is not null)
        {
            //await hubContext.Clients.All.DataRefresh("StreamGroups").ConfigureAwait(false);
            await dataRefreshService.RefreshStreamGroups();
            await Publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
        }

        return APIResponse.Ok;
    }
}
