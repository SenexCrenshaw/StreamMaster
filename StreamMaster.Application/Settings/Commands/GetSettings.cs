namespace StreamMaster.Application.Settings.Commands;

[SMAPI]
public record GetSettings : IRequest<SettingDto>;

internal class GetSettingsHandler(
        IMapper mapper,
        IOptionsMonitor<Setting> intsettings,
        IOptionsMonitor<HLSSettings> inthlssettings,
        IOptionsMonitor<SDSettings> intsdsettings
    ) : IRequestHandler<GetSettings, SettingDto>
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task<SettingDto> Handle(GetSettings request, CancellationToken cancellationToken)
    {
        SettingDto ret = mapper.Map<SettingDto>(settings);

        ret.HLS = inthlssettings.CurrentValue;
        ret.SDSettings = intsdsettings.CurrentValue;

        return await Task.FromResult(ret);
    }
}