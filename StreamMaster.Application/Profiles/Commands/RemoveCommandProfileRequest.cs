namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveCommandProfileRequest(string ProfileName) : IRequest<APIResponse>;

public class RemoveCommandProfileRequestHandler(IOptionsMonitor<CommandProfileDict> intProfileSettings, IDataRefreshService dataRefreshService)
: IRequestHandler<RemoveCommandProfileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(RemoveCommandProfileRequest request, CancellationToken cancellationToken)
    {
        CommandProfileDict profileSettings = intProfileSettings.CurrentValue;

        List<string> badNames = [.. profileSettings.Profiles
            .Where(kvp => kvp.Value.IsReadOnly)
            .Select(kvp => kvp.Key)];

        if (badNames.Contains(request.ProfileName, StringComparer.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"Cannot use name {request.ProfileName}");
        }

        if (profileSettings.Profiles.TryGetValue(request.ProfileName, out CommandProfile? profile))
        {
            profileSettings.RemoveProfile(request.ProfileName);

            SettingsHelper.UpdateSetting(profileSettings);
        }

        await dataRefreshService.RefreshCommandProfiles();
        return APIResponse.Ok;
    }
}