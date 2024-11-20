
namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record AddCommandProfileRequest(string ProfileName, string Command, string Parameters) : IRequest<APIResponse>;
public class AddCommandProfileRequestHandler(ILogger<AddCommandProfileRequest> Logger, IDataRefreshService dataRefreshService, IOptionsMonitor<CommandProfileDict> intProfileSettings)
: IRequestHandler<AddCommandProfileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(AddCommandProfileRequest request, CancellationToken cancellationToken)
    {
        CommandProfileDict profileSettings = intProfileSettings.CurrentValue;

        List<string> badNames = profileSettings.Profiles
            .Where(kvp => kvp.Value.IsReadOnly)
            .Select(kvp => kvp.Key)
            .ToList();

        if (badNames.Contains(request.ProfileName, StringComparer.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"Cannot use name {request.ProfileName}");
        }

        CommandProfile profile = new()
        {
            Command = request.Command,
            Parameters = request.Parameters,
        };
        if (profileSettings.Profiles.TryGetValue(request.ProfileName, out _))
        {
            return APIResponse.ErrorWithMessage("Profile already exists");
        }
        else
        {
            profileSettings.AddProfile(request.ProfileName, profile);
        }

        Logger.LogInformation("AddVideoProfileRequest");

        SettingsHelper.UpdateSetting(profileSettings);
        await dataRefreshService.RefreshCommandProfiles();
        return APIResponse.Ok;
    }
}