namespace StreamMaster.Application.Profiles.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateOutputProfileRequest(string Name, string? NewName) : OutputProfileRequest, IRequest<APIResponse> { }

public class UpdateFileProfileRequestHandler(
    ILogger<UpdateOutputProfileRequest> Logger,
    IOptionsMonitor<OutputProfiles> intprofilesettings,
    IDataRefreshService dataRefreshService,
    IRepositoryWrapper repositoryWrapper
    ) : IRequestHandler<UpdateOutputProfileRequest, APIResponse>
{

    private readonly OutputProfiles profilesettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(UpdateOutputProfileRequest request, CancellationToken cancellationToken)
    {
        if (!profilesettings.OutProfiles.ContainsKey(request.Name))
        {
            return APIResponse.Ok;
        }

        if (request.NewName != null && request.NewName.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage("Cannot use name default");
        }

        List<FieldData> fields = new();
        OutputProfile? existingProfile = null;

        if (!profilesettings.OutProfiles.TryGetValue(request.Name, out existingProfile))
        {
            existingProfile = new OutputProfile();
        }

        if (!string.IsNullOrEmpty(request.TVGName) && request.TVGName != existingProfile.TVGName)
        {
            existingProfile.TVGName = request.TVGName;
            fields.Add(new FieldData(OutputProfile.APIName, request.Name, "TVGName", request.TVGName));
        }

        if (!string.IsNullOrEmpty(request.ChannelId) && request.ChannelId != existingProfile.ChannelId)
        {
            existingProfile.ChannelId = request.ChannelId;
            fields.Add(new FieldData(OutputProfile.APIName, request.Name, "ChannelId", request.ChannelId));
        }

        if (!string.IsNullOrEmpty(request.TVGId) && request.TVGId != existingProfile.TVGId)
        {
            existingProfile.TVGId = request.TVGId;
            fields.Add(new FieldData(OutputProfile.APIName, request.Name, "TVGId", request.TVGId));
        }

        if (!string.IsNullOrEmpty(request.TVGGroup) && request.TVGGroup != existingProfile.TVGGroup)
        {
            existingProfile.TVGGroup = request.TVGGroup;
            fields.Add(new FieldData(OutputProfile.APIName, request.Name, "TVGGroup", request.TVGGroup));
        }

        if (!string.IsNullOrEmpty(request.ChannelNumber) && request.ChannelNumber != existingProfile.ChannelNumber)
        {
            existingProfile.ChannelNumber = request.ChannelNumber;
            fields.Add(new FieldData(OutputProfile.APIName, request.Name, "ChannelNumber", request.ChannelNumber));
        }

        if (!string.IsNullOrEmpty(request.GroupTitle) && request.ChannelId != existingProfile.GroupTitle)
        {
            existingProfile.GroupTitle = request.GroupTitle;
            fields.Add(new FieldData(OutputProfile.APIName, request.Name, "GroupTitle", request.GroupTitle));
        }

        if (request.EnableIcon.HasValue)
        {
            existingProfile.EnableIcon = request.EnableIcon.Value;
        }


        if (!string.IsNullOrEmpty(request.NewName) && request.Name != request.NewName)
        {
            profilesettings.OutProfiles.Remove(request.Name);
            profilesettings.OutProfiles.Add(request.NewName, existingProfile);
            fields.Add(new FieldData(OutputProfile.APIName, request.Name, "Name", request.NewName));
            await repositoryWrapper.SaveAsync();
        }

        await repositoryWrapper.SaveAsync();
        SettingsHelper.UpdateSetting(profilesettings);
        if (fields.Count > 0)
        {
            await dataRefreshService.SetField(fields);
        }
        Logger.LogInformation("UpdateFileProfileRequest");

        //await dataRefreshService.RefreshOutProfiles();
        return APIResponse.Ok;
    }

    //public static void UpdateM3UOutputProfile(M3UOutputProfile existingProfile, M3UOutputProfileRequest requestProfile)
    //{
    //    // Only update if the request profile's property is not empty or default
    //    if (!string.IsNullOrEmpty(requestProfile.TVGName))
    //        existingProfile.TVGName = requestProfile.TVGName;

    //    if (!string.IsNullOrEmpty(requestProfile.ChannelId))
    //        existingProfile.ChannelId = requestProfile.ChannelId;

    //    if (!string.IsNullOrEmpty(requestProfile.TVGId))
    //        existingProfile.TVGId = requestProfile.TVGId;

    //    if (!string.IsNullOrEmpty(requestProfile.TVGGroup))
    //        existingProfile.TVGGroup = requestProfile.TVGGroup;

    //    if (!string.IsNullOrEmpty(requestProfile.ChannelNumber))
    //        existingProfile.ChannelNumber = requestProfile.ChannelNumber;

    //    if (!string.IsNullOrEmpty(requestProfile.GroupTitle))
    //        existingProfile.GroupTitle = requestProfile.GroupTitle;

    //    if (requestProfile.EnableIcon.HasValue)
    //        existingProfile.EnableIcon = requestProfile.EnableIcon.Value;
    //}

}