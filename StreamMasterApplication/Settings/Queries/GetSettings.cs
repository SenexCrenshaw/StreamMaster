using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Settings.Queries;

public record GetSettings : IRequest<SettingDto>;

internal class GetSettingsHandler : IRequestHandler<GetSettings, SettingDto>
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetSettingsHandler(
        IMemoryCache memoryCache,

        IMapper mapper
    )
    {
        _memoryCache = memoryCache;

        _mapper = mapper;
    }

    public async Task<SettingDto> Handle(GetSettings request, CancellationToken cancellationToken)
    {
        Setting setting = FileUtil.GetSetting();
        SettingDto ret = _mapper.Map<SettingDto>(setting);

        List<IconFileDto> icons = _memoryCache.GetIcons(_mapper);

        IconFileDto? defaultIcon = icons.FirstOrDefault(a => a.Source == setting.DefaultIcon);
        if (defaultIcon != null)
        {
            IconFileDto defaultIconDto = _mapper.Map<IconFileDto>(defaultIcon);

            ret.DefaultIconDto = defaultIconDto;
        }

        return ret;
    }
}