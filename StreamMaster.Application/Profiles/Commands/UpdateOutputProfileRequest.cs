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
        if (string.IsNullOrEmpty(request.ProfileName) || !profilesettings.Profiles.ContainsKey(request.ProfileName))
        {
            return APIResponse.Ok;
        }

        if (request.NewName != null)
        {
            if (request.NewName.EqualsIgnoreCase("default"))
            {
                return APIResponse.ErrorWithMessage("Cannot use name default");
            }
            if (request.ProfileName.EqualsIgnoreCase("default"))
            {
                return APIResponse.ErrorWithMessage("Cannot use name default");
            }
        }

        OutputProfile? existingProfile = profilesettings.Profiles[request.ProfileName!];
        if (existingProfile is null)
        {
            return APIResponse.Ok;
        }

        List<FieldData> fields = [];

        //if (!profilesettings.Profiles.TryGetValue(request.ProfileName!, out OutputProfile? existingProfile))
        //{
        //    existingProfile = new OutputProfile();
        //}

        if (!string.IsNullOrEmpty(request.Name) && request.Name != existingProfile.Name)
        {
            existingProfile.Name = request.Name;
            fields.Add(new FieldData(OutputProfile.APIName, existingProfile.Name, "Name", request.Name));
        }

        if (!string.IsNullOrEmpty(request.Id) && request.Id != existingProfile.Id)
        {
            existingProfile.Id = request.Id;
            fields.Add(new FieldData(OutputProfile.APIName, existingProfile.Name, "Id", request.Id));
        }

        //if (!string.IsNullOrEmpty(request.EPGId) && request.EPGId != existingProfile.EPGId)
        //{
        //    existingProfile.EPGId = request.EPGId;
        //    fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "EPGId", request.EPGId));
        //}

        if (!string.IsNullOrEmpty(request.Group) && request.Group != existingProfile.Group)
        {
            existingProfile.Group = request.Group;
            fields.Add(new FieldData(OutputProfile.APIName, existingProfile.Name, "Group", request.Group));
        }

        if (request.EnableChannelNumber.HasValue)
        {
            existingProfile.EnableChannelNumber = request.EnableChannelNumber.Value;
            fields.Add(new FieldData(OutputProfile.APIName, existingProfile.Name, "EnableChannelNumber", request.EnableChannelNumber.Value));
        }

        //if (request.AppendChannelNumberToId.HasValue)
        //{
        //    existingProfile.AppendChannelNumberToId = request.AppendChannelNumberToId.Value;
        //    fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "AppendChannelNumberToId", request.AppendChannelNumberToId.Value));
        //}

        if (request.EnableGroupTitle.HasValue)
        {
            existingProfile.EnableGroupTitle = request.EnableGroupTitle.Value;
            fields.Add(new FieldData(OutputProfile.APIName, existingProfile.Name, "EnableGroupTitle", request.EnableGroupTitle.Value));
        }

        //if (request.EnableId.HasValue)
        //{
        //    existingProfile.EnableId = request.EnableId.Value;
        //    fields.Add(new FieldData(OutputProfile.APIName, request.ProfileName, "EnableId", request.EnableId.Value));
        //}

        if (request.EnableIcon.HasValue)
        {
            existingProfile.EnableIcon = request.EnableIcon.Value;
            fields.Add(new FieldData(OutputProfile.APIName, existingProfile.Name, "EnableIcon", request.EnableIcon.Value));
        }

        bool nameChanged = false;
        if (!string.IsNullOrEmpty(request.NewName) && request.ProfileName != request.NewName)
        {
            nameChanged = true;
            profilesettings.RemoveProfile(request.ProfileName!);
            profilesettings.AddProfile(request.NewName, existingProfile);
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