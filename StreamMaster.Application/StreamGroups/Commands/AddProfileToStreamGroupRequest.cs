namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddProfileToStreamGroupRequest(int StreamGroupId, string ProfileName, string OutputProfileName, string CommandProfileName) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class AddProfileToStreamGroupRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<AddProfileToStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddProfileToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || string.IsNullOrEmpty(request.ProfileName))
        {
            return APIResponse.NotFound;
        }

        if (request.ProfileName.EqualsIgnoreCase("default"))
        {
            return APIResponse.ErrorWithMessage($"The name '{request.ProfileName}' is reserved");
        }

        StreamGroup? streamGroup = Repository.StreamGroup.GetStreamGroup(request.StreamGroupId);
        if (streamGroup is null)
        {
            return APIResponse.ErrorWithMessage("Stream Group not found");
        }


        if (streamGroup.StreamGroupProfiles.Any(x => x.ProfileName == request.ProfileName))
        {
            return APIResponse.ErrorWithMessage("Profile with this name already exists");
        }

        streamGroup.StreamGroupProfiles.Add(new StreamGroupProfile
        {
            ProfileName = request.ProfileName,
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
