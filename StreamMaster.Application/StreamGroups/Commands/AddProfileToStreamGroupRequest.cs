namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddProfileToStreamGroupRequest(int StreamGroupId, string Name, string OutputProfileName, string CommandProfileName) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class AddProfileToStreamGroupRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<AddProfileToStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddProfileToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 2 || string.IsNullOrEmpty(request.Name))
        {
            return APIResponse.NotFound;
        }

        if (request.Name.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage("Cannot use name default");
        }

        StreamGroup? streamGroup = Repository.StreamGroup.GetStreamGroup(request.StreamGroupId);
        if (streamGroup is null)
        {
            return APIResponse.ErrorWithMessage("Stream Group not found");
        }
        if (streamGroup.Name.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage("Cannot use All stream group");
        }

        if (streamGroup.StreamGroupProfiles.Any(x => x.Name == request.Name))
        {
            return APIResponse.ErrorWithMessage("Profile with this name already exists");
        }

        streamGroup.StreamGroupProfiles.Add(new StreamGroupProfile
        {
            Name = request.Name,
            OutputProfileName = request.OutputProfileName,
            CommandProfileName = request.CommandProfileName
        });

        Repository.StreamGroup.Update(streamGroup);
        _ = await Repository.SaveAsync();


        await dataRefreshService.RefreshStreamGroups();

        await messageService.SendSuccess("Profile added successfully");
        return APIResponse.Ok;
    }
}
