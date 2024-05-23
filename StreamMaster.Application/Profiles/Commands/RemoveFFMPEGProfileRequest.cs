namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveFFMPEGProfileRequest(string Name) : IRequest<APIResponse> { }

public class RemoveFFMPEGProfileRequestHandler(IOptionsMonitor<FFMPEGProfiles> intprofilesettings, ILogger<RemoveFFMPEGProfileRequest> Logger, IMapper Mapper)
: IRequestHandler<RemoveFFMPEGProfileRequest, APIResponse>
{
    private readonly FFMPEGProfiles profileSettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(RemoveFFMPEGProfileRequest request, CancellationToken cancellationToken)
    {

        if (profileSettings.Profiles.TryGetValue(request.Name, out FFMPEGProfile? profile))
        {
            profileSettings.Profiles.Remove(request.Name);

            Logger.LogInformation("RemoveFFMPEGProfileRequest");

            SettingsHelper.UpdateSetting(profileSettings);

        }

        SettingDto settingsDto = Mapper.Map<SettingDto>(profileSettings);
        //APIResponse.Success(new UpdateSettingResponse { Settings = settingsDto, NeedsLogOut = false });
        return APIResponse.Ok;

    }

}