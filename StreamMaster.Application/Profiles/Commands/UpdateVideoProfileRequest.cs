namespace StreamMaster.Application.Profiles.Commands;


[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateVideoProfileRequest(string Name, string? NewName, string? Command, string? Parameters, int? Timeout, bool? IsM3U8)
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


        if (!profilesettings.VideoProfiles.ContainsKey(request.Name))
        {
            return APIResponse.ErrorWithMessage($"VideoProfile '" + request.Name + "' doesnt exist"); ;
        }
        List<FieldData> fields = new();

        if (profilesettings.VideoProfiles.TryGetValue(request.Name, out VideoOutputProfile? existingProfile))
        {

            if (request.Command != null && existingProfile.Command != request.Command)
            {
                existingProfile.Command = request.Command;
                fields.Add(new FieldData("GetVideoProfiles", request.Name, "Command", request.Command));
            }
            if (request.Parameters != null && existingProfile.Parameters != request.Parameters)
            {
                existingProfile.Parameters = request.Parameters;
                fields.Add(new FieldData("GetVideoProfiles", request.Name, "Parameters", request.Parameters));
            }
            if (request.Timeout.HasValue && existingProfile.Timeout != request.Timeout.Value)
            {
                existingProfile.Timeout = request.Timeout.Value;
                fields.Add(new FieldData("GetVideoProfiles", request.Name, "Timeout", request.Timeout));
            }
            if (request.IsM3U8.HasValue && request.IsM3U8.Value != existingProfile.IsM3U8)
            {
                existingProfile.IsM3U8 = request.IsM3U8.Value;
                fields.Add(new FieldData("GetVideoProfiles", request.Name, "IsM3U8", request.IsM3U8));
            }
            if (request.NewName != null)
            {
                profilesettings.VideoProfiles.Remove(request.Name);
                profilesettings.VideoProfiles.Add(request.NewName, existingProfile);

            }
            Logger.LogInformation("UpdateVideoProfileRequest");

            SettingsHelper.UpdateSetting(profilesettings);

            if (fields.Count > 0)
            {
                await dataRefreshService.SetField(fields);
            }
        }

        return APIResponse.Ok;
    }

}