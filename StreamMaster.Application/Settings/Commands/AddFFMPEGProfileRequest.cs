using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;

namespace StreamMaster.Application.Settings.Commands;

public record AddFFMPEGProfileRequest(string Name, string Parameters, int TimeOut, bool IsM3U8) : IRequest<UpdateSettingResponse> { }

public class AddFFMPEGProfileRequestHandler(ILogger<AddFFMPEGProfileRequest> Logger, IOptionsMonitor<FFMPEGProfiles> intprofilesettings, IMapper Mapper)
: IRequestHandler<AddFFMPEGProfileRequest, UpdateSettingResponse>
{

    private readonly FFMPEGProfiles profilesettings = intprofilesettings.CurrentValue;

    public async Task<UpdateSettingResponse> Handle(AddFFMPEGProfileRequest request, CancellationToken cancellationToken)
    {

        FFMPEGProfile profile = new()
        {
            Parameters = request.Parameters,
            Timeout = request.TimeOut,
            IsM3U8 = request.IsM3U8
        };

        if (profilesettings.Profiles.TryGetValue(request.Name, out FFMPEGProfile? existingProfile))
        {
            profilesettings.Profiles[request.Name] = profile;
        }
        else
        {
            profilesettings.Profiles.Add(request.Name, profile);
        }


        Logger.LogInformation("AddFFMPEGProfileRequest");

        SettingsHelper.UpdateSetting(profilesettings);


        SettingDto ret = Mapper.Map<SettingDto>(profilesettings);
        return new UpdateSettingResponse { Settings = ret, NeedsLogOut = false };
    }

}