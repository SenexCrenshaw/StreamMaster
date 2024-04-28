namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateStreamGroupRequest(string Name) : IRequest<APIResponse> { }

[LogExecutionTimeAspect]
public class CreateStreamGroupRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IPublisher Publisher, IMapper Mapper)
    : IRequestHandler<CreateStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return APIResponse.NotFound;
        }

        StreamGroup streamGroup = new()
        {
            Name = request.Name,
        };

        Repository.StreamGroup.CreateStreamGroup(streamGroup);
        await Repository.SaveAsync();

        await hubContext.Clients.All.DataRefresh("StreamGroups").ConfigureAwait(false);

        await messageService.SendSuccess("Stream Group '" + request.Name + "' added successfully");
        return APIResponse.Ok;
    }
}
