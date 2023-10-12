namespace StreamMasterApplication.Settings.Queries;

public record GetSettings : IRequest<SettingDto>;

internal class GetSettingsHandler(IMapper mapper, ISettingsService settingsService) : IRequestHandler<GetSettings, SettingDto>
{
    public async Task<SettingDto> Handle(GetSettings request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        return mapper.Map<SettingDto>(setting);
    }
}