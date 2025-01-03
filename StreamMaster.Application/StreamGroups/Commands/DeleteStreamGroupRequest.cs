using StreamMaster.Application.Services;

namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteStreamGroupRequest(int StreamGroupId) : IRequest<APIResponse>;

public class DeleteStreamGroupRequestHandler(IRepositoryWrapper Repository, IBackgroundTaskQueue taskQueue, IDataRefreshService dataRefreshService, IMessageService messageService)
    : IRequestHandler<DeleteStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteStreamGroupRequest request, CancellationToken cancellationToken = default)
    {
        if (request.StreamGroupId < 2)
        {
            await messageService.SendError("Stream Group not found");
            return APIResponse.NotFound;
        }

        StreamGroup? streamGroup = await Repository.StreamGroup.FirstOrDefaultAsync(a => a.Id == request.StreamGroupId, cancellationToken: cancellationToken);
        if (streamGroup == null)
        {
            await messageService.SendError("Stream Group not found");
            return APIResponse.NotFound;
        }

        if (streamGroup.Name.EqualsIgnoreCase("all"))
        {
            return APIResponse.ErrorWithMessage($"Cannot delete reserved '{streamGroup.Name}' streamgroup");
        }

        if (await Repository.StreamGroup.DeleteStreamGroup(request.StreamGroupId) != null)
        {
            _ = await Repository.SaveAsync();

            await dataRefreshService.RefreshStreamGroups();
            await messageService.SendSuccess("Stream Group deleted successfully");

            if (streamGroup.CreateSTRM)
            {
                await taskQueue.CreateSTRMFiles(cancellationToken);
            }
            return APIResponse.Ok;
        }

        return APIResponse.NotFound;
    }
}