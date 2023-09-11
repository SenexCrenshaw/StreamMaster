namespace StreamMasterApplication.Settings.Queries;

public record GetSettings : IRequest<SettingDto>;

internal class GetSettingsHandler(IMapper mapper, ISettingsService settingsService) : IRequestHandler<GetSettings, SettingDto>
{
    public async Task<SettingDto> Handle(GetSettings request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        SettingDto ret = mapper.Map<SettingDto>(setting);

        return ret;
    }
}