using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteStreamGroupRequest(int StreamGroupId) : IRequest<APIResponse>;

public class DeleteStreamGroupRequestHandler(IRepositoryWrapper Repository, IDataRefreshService dataRefreshService, IMessageService messageService, IPublisher Publisher)
    : IRequestHandler<DeleteStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteStreamGroupRequest request, CancellationToken cancellationToken = default)
    {

        if (request.StreamGroupId < 2)
        {
            await messageService.SendError("Stream Group not found");
            return APIResponse.NotFound;
        }

        StreamGroup? streamGroup = await Repository.StreamGroup.FirstOrDefaultAsync(a => a.Id == request.StreamGroupId);
        if (streamGroup == null)
        {
            await messageService.SendError("Stream Group not found");
            return APIResponse.NotFound;
        }

        if (streamGroup.Name.Equals("all", StringComparison.CurrentCultureIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"Cannot delete reserved '{streamGroup.Name}' streamgroup");
        }

        if (await Repository.StreamGroup.DeleteStreamGroup(request.StreamGroupId) != null)
        {
            _ = await Repository.SaveAsync();
            await Publisher.Publish(new StreamGroupDeleteEvent(), cancellationToken);
            await dataRefreshService.RefreshStreamGroups();
            await messageService.SendSuccess("Stream Group deleted successfully");
            return APIResponse.Ok;
        }

        return APIResponse.NotFound;
    }
}
