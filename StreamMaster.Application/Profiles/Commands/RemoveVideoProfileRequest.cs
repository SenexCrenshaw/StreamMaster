namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveVideoProfileRequest(string Name) : IRequest<APIResponse>;

public class RemoveVideoProfileRequestHandler(IOptionsMonitor<VideoOutputProfiles> intProfileSettings, IDataRefreshService dataRefreshService, ILogger<RemoveVideoProfileRequest> Logger, IMapper Mapper)
: IRequestHandler<RemoveVideoProfileRequest, APIResponse>
{

    public async Task<APIResponse> Handle(RemoveVideoProfileRequest request, CancellationToken cancellationToken)
    {
        VideoOutputProfiles profileSettings = intProfileSettings.CurrentValue;

        List<string> badNames = profileSettings.VideoProfiles
            .Where(kvp => kvp.Value.IsReadOnly)
            .Select(kvp => kvp.Key)
            .ToList();

        if (badNames.Contains(request.Name, StringComparer.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"Cannot use name {request.Name}");
        }

        if (profileSettings.VideoProfiles.TryGetValue(request.Name, out VideoOutputProfile? profile))
        {
            _ = profileSettings.VideoProfiles.Remove(request.Name);

            Logger.LogInformation("RemoveVideoProfileRequest");

            SettingsHelper.UpdateSetting(profileSettings);

        }

        await dataRefreshService.RefreshVideoProfiles();
        return APIResponse.Ok;

    }

}