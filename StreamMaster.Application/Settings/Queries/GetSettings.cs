using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.Settings.Queries;

public record GetSettings : IRequest<SettingDto>;

internal class GetSettingsHandler(IMapper mapper, IOptionsMonitor<Setting> intsettings) : IRequestHandler<GetSettings, SettingDto>
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task<SettingDto> Handle(GetSettings request, CancellationToken cancellationToken)
    {
        SettingDto a = mapper.Map<SettingDto>(settings);

        return mapper.Map<SettingDto>(settings);
    }
}