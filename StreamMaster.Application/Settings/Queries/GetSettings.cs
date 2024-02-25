using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.Settings.Queries;

public record GetSettings : IRequest<SettingDto>;

internal class GetSettingsHandler(
        IMapper mapper,
        IOptionsMonitor<Setting> intsettings,
        IOptionsMonitor<HLSSettings> inthlssettings,
        IOptionsMonitor<SDSettings> intsdsettings,
        IOptionsMonitor<FFMPEGProfiles> intprofilesettings
    ) : IRequestHandler<GetSettings, SettingDto>
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task<SettingDto> Handle(GetSettings request, CancellationToken cancellationToken)
    {
        SettingDto ret = mapper.Map<SettingDto>(settings);

        ret.HLS = inthlssettings.CurrentValue;
        ret.SDSettings = intsdsettings.CurrentValue;
        foreach (string key in intprofilesettings.CurrentValue.Profiles.Keys)
        {
            ret.FFMPEGProfiles.Add(new FFMPEGProfileDto
            {
                Name = key,
                Parameters = intprofilesettings.CurrentValue.Profiles[key].Parameters,
                Timeout = intprofilesettings.CurrentValue.Profiles[key].Timeout,
                IsM3U8 = intprofilesettings.CurrentValue.Profiles[key].IsM3U8
            });
        }

        return ret;
    }
}