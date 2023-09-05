using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

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

    public Task<SettingDto> Handle(GetSettings request, CancellationToken cancellationToken)
    {
        Setting setting = FileUtil.GetSetting();
        SettingDto ret = _mapper.Map<SettingDto>(setting);

        return Task.FromResult(ret);
    }
}