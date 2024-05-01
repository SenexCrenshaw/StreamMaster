using StreamMaster.Application.StreamGroups.Events;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Application.StreamGroups.Commands;

[LogExecutionTimeAspect]
public class UpdateStreamGroupRequestHandler(IDataRefreshService dataRefreshService, IRepositoryWrapper Repository, IPublisher Publisher)
    : IRequestHandler<UpdateStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || string.IsNullOrEmpty(request.Name))
        {
            return APIResponse.NotFound;
        }

        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroup(request);
        if (streamGroup is not null)
        {
            //await hubContext.Clients.All.DataRefresh("StreamGroups").ConfigureAwait(false);
            await dataRefreshService.RefreshStreamGroups();
            await Publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
        }

        return APIResponse.Ok;
    }
}
