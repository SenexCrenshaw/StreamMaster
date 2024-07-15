namespace StreamMaster.Application.StreamGroups.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateStreamGroupProfileRequest(int StreamGroupId, string Name, string NewName, string? OutputProfileName, string? VideoProfileName) : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class UpdateStreamGroupProfileRequestHandler(IRepositoryWrapper Repository, IMessageService messageService, IDataRefreshService dataRefreshService)
    : IRequestHandler<UpdateStreamGroupProfileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateStreamGroupProfileRequest request, CancellationToken cancellationToken)
    {
        if (request.StreamGroupId < 1 || string.IsNullOrEmpty(request.Name))
        {
            return APIResponse.NotFound;
        }

        if (request.NewName != null && request.NewName.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage("Cannot use name default");
        }


        StreamGroup? streamGroup = Repository.StreamGroup.GetQuery().FirstOrDefault(a => a.Id == request.StreamGroupId);

        if (streamGroup is null)
        {
            return APIResponse.ErrorWithMessage("Stream Group not found");
        }
        //List<FieldData> fields = new List<FieldData>();

        var streamGroupProfile = streamGroup.StreamGroupProfiles.FirstOrDefault(x => x.Name == request.Name);
        if (streamGroupProfile is null)
        {
            return APIResponse.ErrorWithMessage("Stream Group Profile not found");
        }

        if (!string.IsNullOrEmpty(request.OutputProfileName) && streamGroupProfile.OutputProfileName != request.OutputProfileName)
        {
            streamGroupProfile.OutputProfileName = request.OutputProfileName;
            // fields.Add(new FieldData("GetStreamGroupProfiles", streamGroupProfile.Name, "OutputProfileName", request.OutputProfileName));
        }

        if (!string.IsNullOrEmpty(request.VideoProfileName) && streamGroupProfile.VideoProfileName != request.VideoProfileName)
        {
            streamGroupProfile.VideoProfileName = request.VideoProfileName;
            // fields.Add(new FieldData("GetStreamGroupProfiles", streamGroupProfile.Name, "VideoProfileName", request.VideoProfileName));
        }

        if (!string.IsNullOrEmpty(request.NewName) && streamGroupProfile.Name != request.NewName)
        {
            streamGroupProfile.Name = request.NewName;
            // fields.Add(new FieldData("GetStreamGroupProfiles", streamGroupProfile.Name, "Name", request.NewName));
        }


        //if (fields.Count > 0)
        //{

        Repository.StreamGroup.Update(streamGroup);

        await Repository.SaveAsync();
        //  fields.Add(new FieldData("GetStreamGroups", request.StreamGroupId, "StreamGroupProfiles", streamGroup.StreamGroupProfiles));
        await dataRefreshService.RefreshStreamGroups();
        // await dataRefreshService.SetField(fields);
        //}

        await messageService.SendSuccess("Profile Updated");
        return APIResponse.Ok;
    }
}
