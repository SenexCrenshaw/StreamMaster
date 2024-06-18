namespace StreamMaster.Application.Profiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record RemoveFileProfileRequest(string Name) : IRequest<APIResponse> { }

public class RemoveFileProfileRequestHandler(IOptionsMonitor<FileOutputProfiles> intprofilesettings, IDataRefreshService dataRefreshService, ILogger<RemoveFileProfileRequest> Logger, IMapper Mapper)
: IRequestHandler<RemoveFileProfileRequest, APIResponse>
{
    private readonly FileOutputProfiles profileSettings = intprofilesettings.CurrentValue;

    public async Task<APIResponse> Handle(RemoveFileProfileRequest request, CancellationToken cancellationToken)
    {

        if (profileSettings.FileProfiles.TryGetValue(request.Name, out FileOutputProfile? profile))
        {
            profileSettings.FileProfiles.Remove(request.Name);

            Logger.LogInformation("RemoveFileProfileRequest");

            SettingsHelper.UpdateSetting(profileSettings);
            await dataRefreshService.RefreshFileProfiles();
        }

        //SettingDto settingsDto = Mapper.Map<SettingDto>(profileSettings);
        //APIResponse.Success(new UpdateSettingResponse { Settings = settingsDto, NeedsLogOut = false });
        return APIResponse.Ok;

    }

}