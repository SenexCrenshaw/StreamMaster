namespace StreamMaster.Application.Profiles.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateFileProfileRequest(string Name, string? NewName, bool? IsReadOnly, EPGOutputProfileRequest? EPGOutputProfile, M3UOutputProfileRequest? M3UOutputProfile)
    : IRequest<APIResponse>
{ }

public class UpdateFileProfileRequestHandler(
    ILogger<UpdateFileProfileRequest> Logger,
    IOptionsMonitor<FileOutputProfiles> intprofilesettings,
   IDataRefreshService dataRefreshService,
    IRepositoryWrapper repositoryWrapper
    )
: IRequestHandler<UpdateFileProfileRequest, APIResponse>
{

    private readonly FileOutputProfiles profilesettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(UpdateFileProfileRequest request, CancellationToken cancellationToken)
    {
        if (!profilesettings.FileProfiles.ContainsKey(request.Name))
        {
            return APIResponse.Ok;
        }

        List<FieldData> fields = new();

        if (profilesettings.FileProfiles.TryGetValue(request.Name, out FileOutputProfile? existingProfile))
        {

            if (request.EPGOutputProfile != null)
            {
                //existingProfile.EPGOutputProfile = request.EPGOutputProfile;
                //fields.Add(new FieldData("GetFileProfiles", request.Name, "EPGOutputProfile", request.EPGOutputProfile));
            }

            if (request.M3UOutputProfile != null)
            {
                UpdateM3UOutputProfile(existingProfile.M3UOutputProfile, request.M3UOutputProfile);
                fields.Add(new FieldData("GetFileProfiles", request.Name, "M3UOutputProfile", existingProfile.M3UOutputProfile));
            }

            if (!string.IsNullOrEmpty(request.NewName) && request.Name != request.NewName)
            {
                profilesettings.FileProfiles.Remove(request.Name);
                profilesettings.FileProfiles.Add(request.NewName, existingProfile);
                fields.Add(new FieldData("GetFileProfiles", request.Name, "Name", request.NewName));
                //repositoryWrapper.StreamGroup.GetQuery(x => x.FileProfileId == request.Name).ToList().ForEach(x => x.FileProfileId = request.NewName);
                await repositoryWrapper.SaveAsync();
            }

            SettingsHelper.UpdateSetting(profilesettings);
            if (fields.Count > 0)
            {
                await dataRefreshService.SetField(fields);
            }
            Logger.LogInformation("UpdateFileProfileRequest");

            //await dataRefreshService.RefreshFileProfiles();
        }

        return APIResponse.Ok;
    }

    public static void UpdateM3UOutputProfile(M3UOutputProfile existingProfile, M3UOutputProfileRequest requestProfile)
    {
        // Only update if the request profile's property is not empty or default
        if (!string.IsNullOrEmpty(requestProfile.TVGName))
            existingProfile.TVGName = requestProfile.TVGName;

        if (!string.IsNullOrEmpty(requestProfile.ChannelId))
            existingProfile.ChannelId = requestProfile.ChannelId;

        if (!string.IsNullOrEmpty(requestProfile.TVGId))
            existingProfile.TVGId = requestProfile.TVGId;

        if (!string.IsNullOrEmpty(requestProfile.TVGGroup))
            existingProfile.TVGGroup = requestProfile.TVGGroup;

        if (!string.IsNullOrEmpty(requestProfile.ChannelNumber))
            existingProfile.ChannelNumber = requestProfile.ChannelNumber;

        if (!string.IsNullOrEmpty(requestProfile.GroupTitle))
            existingProfile.GroupTitle = requestProfile.GroupTitle;

        if (requestProfile.EnableIcon.HasValue)
            existingProfile.EnableIcon = requestProfile.EnableIcon.Value;
    }

}