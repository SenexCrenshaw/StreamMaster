namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateCommandProfileRequest(string ProfileName, string? NewProfileName, string? Command, string? Parameters)
    : IRequest<APIResponse>;

public class UpdateVideoProfileRequestHandler(
    ILogger<UpdateCommandProfileRequest> Logger,
    IOptionsMonitor<CommandProfileDict> intProfileSettings,
    IDataRefreshService dataRefreshService
    ) : IRequestHandler<UpdateCommandProfileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateCommandProfileRequest request, CancellationToken cancellationToken)
    {
        if (!intProfileSettings.CurrentValue.Profiles.ContainsKey(request.ProfileName))
        {
            return APIResponse.ErrorWithMessage("CommandProfile '" + request.ProfileName + "' doesnt exist");
        }

        if (!string.IsNullOrEmpty(request.NewProfileName))
        {
            CommandProfileDict profileSettings = intProfileSettings.CurrentValue;

            List<string> badNames = [.. profileSettings.Profiles
                .Where(kvp => kvp.Value.IsReadOnly)
                .Select(kvp => kvp.Key)];

            if (badNames.Contains(request.NewProfileName, StringComparer.OrdinalIgnoreCase))
            {
                return APIResponse.ErrorWithMessage($"Cannot use name {request.NewProfileName}");
            }
        }

        List<FieldData> fields = [];

        if (intProfileSettings.CurrentValue.Profiles.TryGetValue(request.ProfileName, out CommandProfile? existingProfile))
        {
            if (request.Command != null && existingProfile.Command != request.Command)
            {
                existingProfile.Command = request.Command;
                fields.Add(new FieldData("GetCommandProfiles", request.ProfileName, "Command", request.Command));
            }
            if (request.Parameters != null && existingProfile.Parameters != request.Parameters)
            {
                existingProfile.Parameters = request.Parameters;
                fields.Add(new FieldData("GetCommandProfiles", request.ProfileName, "Parameters", request.Parameters));

                bool nameChanged = false;
                if (request.NewProfileName != null)
                {
                    nameChanged = true;
                    intProfileSettings.CurrentValue.RemoveProfile(request.ProfileName);
                    intProfileSettings.CurrentValue.AddProfile(request.NewProfileName, existingProfile);
                }
                Logger.LogInformation("UpdateVideoProfileRequest");

                SettingsHelper.UpdateSetting(intProfileSettings.CurrentValue);
                if (nameChanged)
                {
                    await dataRefreshService.RefreshCommandProfiles();
                }
                else
                {
                    if (fields.Count > 0)
                    {
                        await dataRefreshService.SetField(fields);
                    }
                }
            }
        }
        return APIResponse.Ok;
    }
}
