namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateStreamGroupProfileRequest(int StreamGroupId, string ProfileName, string NewProfileName, string? OutputProfileName, string? CommandProfileName) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class UpdateStreamGroupProfileRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<UpdateStreamGroupProfileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateStreamGroupProfileRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || string.IsNullOrEmpty(request.ProfileName))
        {
            return APIResponse.NotFound;
        }

        if (request.NewProfileName?.EqualsIgnoreCase("all") == true)
        {
            return APIResponse.ErrorWithMessage($"The New Profile Name '{request.NewProfileName}' is reserved");
        }
        if (request.NewProfileName?.EqualsIgnoreCase("default") == true)
        {
            return APIResponse.ErrorWithMessage($"The New Profile Name '{request.NewProfileName}' is reserved");
        }

        StreamGroup? streamGroup = Repository.StreamGroup.GetQuery().FirstOrDefault(a => a.Id == request.StreamGroupId);

        if (streamGroup is null)
        {
            return APIResponse.ErrorWithMessage("Stream Group not found");
        }

        StreamGroupProfile? streamGroupProfile = streamGroup.StreamGroupProfiles.Find(x => x.ProfileName == request.ProfileName);
        if (streamGroupProfile is null)
        {
            return APIResponse.ErrorWithMessage("Stream Group Profile not found");
        }

        if (!string.IsNullOrEmpty(request.OutputProfileName) && streamGroupProfile.OutputProfileName != request.OutputProfileName)
        {
            streamGroupProfile.OutputProfileName = request.OutputProfileName;
            // fields.Add(new FieldData("GetStreamGroupProfiles", streamGroupProfile.ProfileName, "OutputProfileName", request.OutputProfileName));
        }

        if (!string.IsNullOrEmpty(request.CommandProfileName) && streamGroupProfile.CommandProfileName != request.CommandProfileName)
        {
            streamGroupProfile.CommandProfileName = request.CommandProfileName;
            // fields.Add(new FieldData("GetStreamGroupProfiles", streamGroupProfile.ProfileName, "CommandProfileName", request.CommandProfileName));
        }

        if (!string.IsNullOrEmpty(request.NewProfileName) && streamGroupProfile.ProfileName != request.NewProfileName)
        {
            streamGroupProfile.ProfileName = request.NewProfileName;
            // fields.Add(new FieldData("GetStreamGroupProfiles", streamGroupProfile.ProfileName, "ProfileName", request.NewName));
        }

        //if (fields.Count > 0)
        //{

        Repository.StreamGroup.Update(streamGroup);

        _ = await Repository.SaveAsync();
        //  fields.Add(new FieldData("GetStreamGroups", request.StreamGroupId, "StreamGroupProfiles", streamGroup.StreamGroupProfiles));
        await dataRefreshService.RefreshStreamGroups();
        // await dataRefreshService.SetField(fields);
        //}

        await messageService.SendSuccess("Profile Updated");
        return APIResponse.Ok;
    }
}
