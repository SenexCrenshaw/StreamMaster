using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.Services;

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
        var ret = _mapper.Map<SettingDto>(setting);

        if (_memoryCache.TryGetValue(CacheKeys.ListIconFiles, out List<IconFileDto>? cacheValue))
        {
            if (cacheValue != null)
            {
                var defaultIcon = cacheValue.FirstOrDefault(a => a.Source == setting.DefaultIcon);
                if (defaultIcon != null)
                {
                    IconFileDto defaultIconDto = _mapper.Map<IconFileDto>(defaultIcon);

                    ret.DefaultIconDto = defaultIconDto;
                }
            }
        }

        return ret;
    }
}
