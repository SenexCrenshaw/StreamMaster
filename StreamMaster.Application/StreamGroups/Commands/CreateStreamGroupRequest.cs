namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record CreateStreamGroupRequest(string Name) : IRequest<APIResponse> { }

[LogExecutionTimeAspect]
public class CreateStreamGroupRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<CreateStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(CreateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return APIResponse.NotFound;
        }


        if (request.Name.Equals("all", StringComparison.CurrentCultureIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"The name '{request.Name}' is reserved");
        }

        StreamGroup streamGroup = new()
        {
            Name = request.Name,

        };

        streamGroup.StreamGroupProfiles.Add(new StreamGroupProfile
        {
            Name = "Default",
            OutputProfileName = "Default",
            VideoProfileName = "Default"
        });

        Repository.StreamGroup.CreateStreamGroup(streamGroup);
        await Repository.SaveAsync();

        await dataRefreshService.RefreshStreamGroups();

        await messageService.SendSuccess("Stream Group '" + request.Name + "' added successfully");
        return APIResponse.Ok;
    }
}
