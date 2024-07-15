using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateStreamGroupRequest(int StreamGroupId, string? NewName, string? DeviceID, List<string>? StreamGroupProfiles)
 : IRequest<APIResponse>
{ }


[LogExecutionTimeAspect]
public class UpdateStreamGroupRequestHandler(IDataRefreshService dataRefreshService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<UpdateStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return APIResponse.NotFound;
        }

        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroup(request.StreamGroupId, request.NewName, request.DeviceID);//, request.AutoSetChannelNumbers, request.IgnoreExistingChannelNumbers, request.StartingChannelNumber);
        if (streamGroup is not null)
        {
            //await hubContext.Clients.All.DataRefresh("StreamGroups").ConfigureAwait(false);
            await dataRefreshService.RefreshStreamGroups();
            await Publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
        }

        return APIResponse.Ok;
    }
}
