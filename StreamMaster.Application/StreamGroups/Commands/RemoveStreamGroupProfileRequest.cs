namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveStreamGroupProfileRequest(int StreamGroupId, string ProfileName) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class RemoveStreamGroupProfileRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<RemoveStreamGroupProfileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveStreamGroupProfileRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || string.IsNullOrEmpty(request.ProfileName))
        {
            return APIResponse.NotFound;
        }

        if (request.ProfileName.EqualsIgnoreCase("default"))
        {
            return APIResponse.ErrorWithMessage($"The Profile Name '{request.ProfileName}' is reserved");
        }

        StreamGroup? streamGroup = Repository.StreamGroup.GetStreamGroup(request.StreamGroupId);
        if (streamGroup is null)
        {
            return APIResponse.ErrorWithMessage("Stream Group not found");
        }

        StreamGroupProfile? test = Repository.StreamGroupProfile.GetQuery().FirstOrDefault(a => a.StreamGroupId == streamGroup.Id && a.ProfileName == request.ProfileName);
        if (test is not null)
        {
            await Repository.StreamGroupProfile.DeleteStreamGroupProfile(test);
            _ = await Repository.SaveAsync();
            await dataRefreshService.RefreshStreamGroups();
        }

        await messageService.SendSuccess("Profile removed successfully");
        return APIResponse.Ok;
    }
}
