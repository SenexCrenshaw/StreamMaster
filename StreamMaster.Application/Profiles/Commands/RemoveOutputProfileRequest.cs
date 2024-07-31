namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveOutputProfileRequest(string Name) : IRequest<APIResponse>;

public class RemoveOutputProfileRequestHandler(IOptionsMonitor<OutputProfiles> intProfileSettings, IDataRefreshService dataRefreshService, ILogger<RemoveOutputProfileRequest> Logger, IMapper Mapper)
: IRequestHandler<RemoveOutputProfileRequest, APIResponse>
{


    public async Task<APIResponse> Handle(RemoveOutputProfileRequest request, CancellationToken cancellationToken)
    {
        OutputProfiles profileSettings = intProfileSettings.CurrentValue;

        List<string> badNames = profileSettings.Profiles
            .Where(kvp => kvp.Value.IsReadOnly)
            .Select(kvp => kvp.Key)
            .ToList();


        if (badNames.Contains(request.Name, StringComparer.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage($"Cannot use name {request.Name}");

        }
        if (profileSettings.Profiles.TryGetValue(request.Name, out OutputProfile? profile))
        {
            _ = profileSettings.Profiles.Remove(request.Name);

            Logger.LogInformation("RemoveFileProfileRequest");

            SettingsHelper.UpdateSetting(profileSettings);
            await dataRefreshService.RefreshOutputProfiles();
        }

        //SettingDto settingsDto = Mapper.Map<SettingDto>(profileSettings);
        //APIResponse.Success(new UpdateSettingResponse { Settings = settingsDto, NeedsLogOut = false });
        return APIResponse.Ok;

    }

}