namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveStreamGroupProfileRequest(int StreamGroupId, string Name) : IRequest<APIResponse> { }

[LogExecutionTimeAspect]
public class RemoveStreamGroupProfileRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<RemoveStreamGroupProfileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveStreamGroupProfileRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || string.IsNullOrEmpty(request.Name))
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


        var test = Repository.StreamGroupProfile.GetStreamGroupProfiles().Find(a => a.StreamGroupId == streamGroup.Id && a.Name == request.Name);
        if (test is not null)
        {
            Repository.StreamGroupProfile.DeleteStreamGroupProfile(test);
            await Repository.SaveAsync();
            await dataRefreshService.RefreshStreamGroups();
        }



        await messageService.SendSuccess("Profile removed successfully");
        return APIResponse.Ok;
    }
}
