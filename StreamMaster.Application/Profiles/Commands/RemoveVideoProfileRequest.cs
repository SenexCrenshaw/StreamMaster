namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveVideoProfileRequest(string Name) : IRequest<APIResponse>;

public class RemoveVideoProfileRequestHandler(IOptionsMonitor<VideoOutputProfiles> intprofilesettings, IDataRefreshService dataRefreshService, ILogger<RemoveVideoProfileRequest> Logger, IMapper Mapper)
: IRequestHandler<RemoveVideoProfileRequest, APIResponse>
{
    private readonly VideoOutputProfiles profileSettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(RemoveVideoProfileRequest request, CancellationToken cancellationToken)
    {
        if (request.Name.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage("Cannot use name default");
        }

        if (profileSettings.VideoProfiles.TryGetValue(request.Name, out VideoOutputProfile? profile))
        {
            profileSettings.VideoProfiles.Remove(request.Name);

            Logger.LogInformation("RemoveVideoProfileRequest");

            SettingsHelper.UpdateSetting(profileSettings);

        }

        await dataRefreshService.RefreshVideoProfiles();
        return APIResponse.Ok;

    }

}