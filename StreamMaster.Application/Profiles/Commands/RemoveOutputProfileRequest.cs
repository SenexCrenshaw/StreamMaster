namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveOutputProfileRequest(string Name) : IRequest<APIResponse>;

public class RemoveOutputProfileRequestHandler(IOptionsMonitor<OutputProfiles> intprofilesettings, IDataRefreshService dataRefreshService, ILogger<RemoveOutputProfileRequest> Logger, IMapper Mapper)
: IRequestHandler<RemoveOutputProfileRequest, APIResponse>
{
    private readonly OutputProfiles profileSettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(RemoveOutputProfileRequest request, CancellationToken cancellationToken)
    {
        if (request.Name.Equals("default", StringComparison.OrdinalIgnoreCase))
        {
            return APIResponse.ErrorWithMessage("Cannot use name default");
        }

        if (profileSettings.OutProfiles.TryGetValue(request.Name, out OutputProfile? profile))
        {
            profileSettings.OutProfiles.Remove(request.Name);

            Logger.LogInformation("RemoveFileProfileRequest");

            SettingsHelper.UpdateSetting(profileSettings);
            await dataRefreshService.RefreshOutputProfiles();
        }

        //SettingDto settingsDto = Mapper.Map<SettingDto>(profileSettings);
        //APIResponse.Success(new UpdateSettingResponse { Settings = settingsDto, NeedsLogOut = false });
        return APIResponse.Ok;

    }

}