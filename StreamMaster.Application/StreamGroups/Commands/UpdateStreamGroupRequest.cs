using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateStreamGroupRequest(int StreamGroupId, string? NewName, string? DeviceID, List<string>? StreamGroupProfiles)
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

        if (request.NewName?.Equals("default", StringComparison.OrdinalIgnoreCase) == true)
        {
            return APIResponse.ErrorWithMessage("Cannot use name default");
        }

        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroup(request.StreamGroupId, request.StreamGroupId == 1 ? null : request.NewName, request.DeviceID);
        if (streamGroup is not null)
        {
            await dataRefreshService.RefreshStreamGroups();
            await Publisher.Publish(new StreamGroupUpdateEvent(streamGroup), cancellationToken).ConfigureAwait(false);
        }

        return APIResponse.Ok;
    }
}
