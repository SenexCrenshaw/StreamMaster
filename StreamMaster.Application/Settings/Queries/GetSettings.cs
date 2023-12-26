using StreamMaster.Domain.Common;
using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Services;

namespace StreamMaster.Application.Settings.Queries;

public record GetSettings : IRequest<SettingDto>;

internal class GetSettingsHandler(IMapper mapper, ISettingsService settingsService) : IRequestHandler<GetSettings, SettingDto>
{
    public async Task<SettingDto> Handle(GetSettings request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        return mapper.Map<SettingDto>(setting);
    }
}