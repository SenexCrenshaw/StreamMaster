using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateStreamGroupRequest(int StreamGroupId, string? GroupKey, string? NewName, string? DeviceID)
 : IRequest<APIResponse>;

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

        if (request.NewName != null && request.NewName.Equals("all", StringComparison.CurrentCultureIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"The name '{request.NewName}' is reserved");
        }

        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroup(request.StreamGroupId, request.StreamGroupId == 1 ? null : request.NewName, request.DeviceID, request.GroupKey);
        if (streamGroup is not null)
        {
            await dataRefreshService.RefreshStreamGroups();
            await Publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
        }

        return APIResponse.Ok;
    }
}
