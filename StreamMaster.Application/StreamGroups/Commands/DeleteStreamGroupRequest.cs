using StreamMaster.Application.StreamGroups.Events;

namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record DeleteStreamGroupRequest(int Id) : IRequest<APIResponse> { }

public class DeleteStreamGroupRequestHandler(IRepositoryWrapper Repository, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMessageService messageService, IPublisher Publisher)
    : IRequestHandler<DeleteStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(DeleteStreamGroupRequest request, CancellationToken cancellationToken = default)
    {

        if (request.Id < 1)
        {
            await messageService.SendError("Stream Group not found");
            return APIResponse.NotFound;
        }

        if (await Repository.StreamGroup.DeleteStreamGroup(request.Id) != null)
        {
            await Repository.SaveAsync();
            await Publisher.Publish(new StreamGroupDeleteEvent(), cancellationToken);
            await hubContext.Clients.All.DataRefresh("StreamGroups").ConfigureAwait(false);
            await messageService.SendSuccess("Stream Group deleted successfully");
            return APIResponse.Ok;
        }

        return APIResponse.NotFound;
    }
}
