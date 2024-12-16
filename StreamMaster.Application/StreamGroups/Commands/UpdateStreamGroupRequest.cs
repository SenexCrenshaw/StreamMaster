using StreamMaster.Application.Services;

namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateStreamGroupRequest(int StreamGroupId, string? GroupKey, string? NewName, string? DeviceID, bool? CreateSTRM)
 : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class UpdateStreamGroupRequestHandler(IDataRefreshService dataRefreshService, IBackgroundTaskQueue taskQueue, IRepositoryWrapper Repository)
    : IRequestHandler<UpdateStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1)
        {
            return APIResponse.NotFound;
        }

        if (request.NewName?.EqualsIgnoreCase("all") == true)
        {
            return APIResponse.ErrorWithMessage($"The name '{request.NewName}' is reserved");
        }

        StreamGroupDto? streamGroup = await Repository.StreamGroup.UpdateStreamGroup(request.StreamGroupId, request.StreamGroupId == 1 ? null : request.NewName, request.DeviceID, request.GroupKey, request.CreateSTRM);
        if (streamGroup is not null)
        {
            await dataRefreshService.RefreshStreamGroups();

            if (request.CreateSTRM.HasValue)
            {
                await taskQueue.CreateSTRMFiles(cancellationToken);
            }
        }

        return APIResponse.Ok;
    }
}