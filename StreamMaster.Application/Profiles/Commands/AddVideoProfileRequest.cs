namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddVideoProfileRequest(string Name, string Command, string Parameters, int Timeout, bool IsM3U8) : IRequest<APIResponse>;

public class AddVideoVideoProfileRequestHandler(ILogger<AddVideoProfileRequest> Logger, IDataRefreshService dataRefreshService, IOptionsMonitor<VideoOutputProfiles> intProfileSettings)
: IRequestHandler<AddVideoProfileRequest, APIResponse>
{


    public async Task<APIResponse> Handle(AddVideoProfileRequest request, CancellationToken cancellationToken)
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

        VideoOutputProfile profile = new()
        {
            Command = request.Command,
            Parameters = request.Parameters,
            Timeout = request.Timeout,
            IsM3U8 = request.IsM3U8
        };
        if (profileSettings.VideoProfiles.TryGetValue(request.Name, out _))
        {
            return APIResponse.ErrorWithMessage("Profile already exists");
        }
        else
        {
            profileSettings.VideoProfiles.Add(request.Name, profile);
        }


        Logger.LogInformation("AddVideoProfileRequest");

        SettingsHelper.UpdateSetting(profileSettings);
        await dataRefreshService.RefreshVideoProfiles();
        return APIResponse.Ok;
    }

}