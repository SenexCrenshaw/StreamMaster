namespace StreamMaster.Application.Profiles.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateOutputProfileRequest(string ProfileName, string? NewName) : OutputProfileRequest, IRequest<APIResponse>;

public class UpdateFileProfileRequestHandler(
    ILogger<UpdateOutputProfileRequest> Logger,
    IOptionsMonitor<OutputProfileDict> intprofilesettings,
    IDataRefreshService dataRefreshService
    ) : IRequestHandler<UpdateOutputProfileRequest, APIResponse>
{

    private readonly OutputProfileDict profilesettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(UpdateOutputProfileRequest request, CancellationToken cancellationToken)
    {
        if (!profilesettings.OutputProfiles.ContainsKey(request.ProfileName))
        {
            return APIResponse.Ok;
        }

        if (request.NewName != null)
        {
            if (request.NewName.Equals("default", StringComparison.OrdinalIgnoreCase))
            {
                return APIResponse.ErrorWithMessage("Cannot use name default");
            }
            if (request.ProfileName != null && request.ProfileName.Equals("default", StringComparison.OrdinalIgnoreCase))
            {
                return APIResponse.ErrorWithMessage("Cannot use name default");
            }

        }

        List<FieldData> fields = [];

        if (!profilesettings.OutputProfiles.TryGetValue(request.ProfileName, out OutputProfile? existingProfile))
        {
            existingProfile = new OutputProfile();
        }

        if (!string.IsNullOrEmpty(request.Name) && request.Name != existingProfile.Name)
        {
            existingProfile.Name = request.Name;
            fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "Name", request.Name));
        }

        if (!string.IsNullOrEmpty(request.Id) && request.Id != existingProfile.Id)
        {
            existingProfile.Id = request.Id;
            fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "Id", request.Id));
        }

        //if (!string.IsNullOrEmpty(request.EPGId) && request.EPGId != existingProfile.EPGId)
        //{
        //    existingProfile.EPGId = request.EPGId;
        //    fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "EPGId", request.EPGId));
        //}

        if (!string.IsNullOrEmpty(request.Group) && request.Group != existingProfile.Group)
        {
            existingProfile.Group = request.Group;
            fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "Group", request.Group));
        }

        if (request.EnableChannelNumber.HasValue)
        {
            existingProfile.EnableChannelNumber = request.EnableChannelNumber.Value;
            fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "EnableChannelNumber", request.EnableChannelNumber.Value));
        }

        //if (request.AppendChannelNumberToId.HasValue)
        //{
        //    existingProfile.AppendChannelNumberToId = request.AppendChannelNumberToId.Value;
        //    fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "AppendChannelNumberToId", request.AppendChannelNumberToId.Value));
        //}

        if (request.EnableGroupTitle.HasValue)
        {
            existingProfile.EnableGroupTitle = request.EnableGroupTitle.Value;
            fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "EnableGroupTitle", request.EnableGroupTitle.Value));
        }

        //if (request.EnableId.HasValue)
        //{
        //    existingProfile.EnableId = request.EnableId.Value;
        //    fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "EnableId", request.EnableId.Value));
        //}

        if (request.EnableIcon.HasValue)
        {
            existingProfile.EnableIcon = request.EnableIcon.Value;
            fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "EnableIcon", request.EnableIcon.Value));
        }

        bool nameChanged = false;
        if (!string.IsNullOrEmpty(request.NewName) && request.ProfileName != request.NewName)
        {
            nameChanged = true;
            _ = profilesettings.OutputProfiles.Remove(request.ProfileName);
            profilesettings.OutputProfiles.Add(request.NewName, existingProfile);
        }

        SettingsHelper.UpdateSetting(profilesettings);
        if (nameChanged || fields.Count > 0)
        {
            await dataRefreshService.RefreshOutputProfiles();
        }
        //else
        //{
        //    if (fields.Count > 0)
        //    {
        //        await dataRefreshService.SetField(fields);
        //    }
        //}
        Logger.LogInformation("UpdateFileProfileRequest");

        //await dataRefreshService.RefreshOutProfiles();
        return APIResponse.Ok;
    }

}