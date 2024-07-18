namespace StreamMaster.Application.Profiles.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateVideoProfileRequest(string ProfileName, string? NewName, string? Command, string? Parameters, int? Timeout, bool? IsM3U8)
    : IRequest<APIResponse>
{ }

public class UpdateVideoProfileRequestHandler(
    ILogger<UpdateVideoProfileRequest> Logger,
    IOptionsMonitor<VideoOutputProfiles> intprofilesettings,
    IDataRefreshService dataRefreshService
    )
: IRequestHandler<UpdateVideoProfileRequest, APIResponse>
{

    private readonly VideoOutputProfiles profilesettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(UpdateVideoProfileRequest request, CancellationToken cancellationToken)
    {


        if (!profilesettings.VideoProfiles.ContainsKey(request.ProfileName))
        {
            return APIResponse.ErrorWithMessage($"VideoProfile '" + request.ProfileName + "' doesnt exist"); ;
        }

        if (request.NewName != null && request.NewName.Equals("DefaultFFmpeg", StringComparison.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage("Cannot use name DefaultFFmpeg");
        }


        List<FieldData> fields = [];

        if (profilesettings.VideoProfiles.TryGetValue(request.ProfileName, out VideoOutputProfile? existingProfile))
        {

            if (request.Command != null && existingProfile.Command != request.Command)
            {
                existingProfile.Command = request.Command;
                fields.Add(new FieldData("GetVideoProfiles", request.ProfileName, "Command", request.Command));
            }
            if (request.Parameters != null && existingProfile.Parameters != request.Parameters)
            {
                existingProfile.Parameters = request.Parameters;
                fields.Add(new FieldData("GetVideoProfiles", request.ProfileName, "Parameters", request.Parameters));
            }
            if (request.Timeout.HasValue && existingProfile.Timeout != request.Timeout.Value)
            {
                existingProfile.Timeout = request.Timeout.Value;
                fields.Add(new FieldData("GetVideoProfiles", request.ProfileName, "Timeout", request.Timeout));
            }
            if (request.IsM3U8.HasValue && request.IsM3U8.Value != existingProfile.IsM3U8)
            {
                existingProfile.IsM3U8 = request.IsM3U8.Value;
                fields.Add(new FieldData("GetVideoProfiles", request.ProfileName, "IsM3U8", request.IsM3U8));
            }

            bool nameChanged = false;
            if (request.NewName != null)
            {
                nameChanged = true;
                _ = profilesettings.VideoProfiles.Remove(request.ProfileName);
                profilesettings.VideoProfiles.Add(request.NewName, existingProfile);

            }
            Logger.LogInformation("UpdateVideoProfileRequest");

            SettingsHelper.UpdateSetting(profilesettings);
            if (nameChanged)
            {
                await dataRefreshService.RefreshVideoProfiles();
            }
            else
            {
                if (fields.Count > 0)
                {
                    await dataRefreshService.SetField(fields);
                }
            }
        }

        return APIResponse.Ok;
    }

}