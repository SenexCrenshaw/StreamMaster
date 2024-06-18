namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddProfileToStreamGroupRequest(int StreamGroupId, string Name, string FileProfileName, string VideoProfileName) : IRequest<APIResponse> { }

[LogExecutionTimeAspect]
public class AddProfileToStreamGroupRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<AddProfileToStreamGroupRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddProfileToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || string.IsNullOrEmpty(request.Name))
        {
            return APIResponse.NotFound;
        }

        StreamGroup? streamGroup = Repository.StreamGroup.GetStreamGroup(request.StreamGroupId);
        if (streamGroup is null)
        {
            return APIResponse.ErrorWithMessage("Stream Group not found");
        }

        if (streamGroup.StreamGroupProfiles.Any(x => x.Name == request.Name))
        {
            return APIResponse.ErrorWithMessage("Profile with this name already exists");
        }

        streamGroup.StreamGroupProfiles.Add(new StreamGroupProfile
        {
            Name = request.Name,
            FileProfileName = request.FileProfileName,
            VideoProfileName = request.VideoProfileName
        });

        Repository.StreamGroup.Update(streamGroup);
        await Repository.SaveAsync();


        await dataRefreshService.RefreshStreamGroups();

        await messageService.SendSuccess("Profile added successfully");
        return APIResponse.Ok;
    }
}
