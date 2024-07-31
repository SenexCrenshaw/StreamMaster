namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveCommandProfileRequest(string ProfileName) : IRequest<APIResponse>;

public class RemoveCommandProfileRequestHandler(IOptionsMonitor<CommandProfiles> intProfileSettings, IDataRefreshService dataRefreshService, ILogger<RemoveCommandProfileRequest> Logger, IMapper Mapper)
: IRequestHandler<RemoveCommandProfileRequest, APIResponse>
{

    public async Task<APIResponse> Handle(RemoveCommandProfileRequest request, CancellationToken cancellationToken)
    {
        CommandProfiles profileSettings = intProfileSettings.CurrentValue;

        List<string> badNames = profileSettings.Profiles
            .Where(kvp => kvp.Value.IsReadOnly)
            .Select(kvp => kvp.Key)
            .ToList();

        if (badNames.Contains(request.ProfileName, StringComparer.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"Cannot use name {request.ProfileName}");
        }

        if (profileSettings.Profiles.TryGetValue(request.ProfileName, out CommandProfile? profile))
        {
            _ = profileSettings.Profiles.Remove(request.ProfileName);

            SettingsHelper.UpdateSetting(profileSettings);

        }

        await dataRefreshService.RefreshCommandProfiles();
        return APIResponse.Ok;

    }

}