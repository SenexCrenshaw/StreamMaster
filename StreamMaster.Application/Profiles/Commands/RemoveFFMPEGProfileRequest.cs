namespace StreamMaster.Application.Profiles.Commands;

public record RemoveFFMPEGProfileRequest(string Name) : IRequest<UpdateSettingResponse> { }

public class RemoveFFMPEGProfileRequestHandler(IOptionsMonitor<FFMPEGProfiles> intprofilesettings, ILogger<RemoveFFMPEGProfileRequest> Logger, IMapper Mapper)
: IRequestHandler<RemoveFFMPEGProfileRequest, UpdateSettingResponse>
{
    private readonly FFMPEGProfiles profilesettings = intprofilesettings.CurrentValue;

    public async Task<UpdateSettingResponse> Handle(RemoveFFMPEGProfileRequest request, CancellationToken cancellationToken)
    {

        if (profilesettings.Profiles.TryGetValue(request.Name, out FFMPEGProfile? profile))
        {
            profilesettings.Profiles.Remove(request.Name);

            Logger.LogInformation("RemoveFFMPEGProfileRequest");

            SettingsHelper.UpdateSetting(profilesettings);

        }

        SettingDto retNull = Mapper.Map<SettingDto>(profilesettings);
        return new UpdateSettingResponse { Settings = retNull, NeedsLogOut = false };


    }

}