namespace StreamMaster.Application.Settings.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSettingsRequest : IRequest<APIResponse<SettingDto>>;

internal class GetSettingsRequestHandler(
        IMapper mapper,
        IOptionsMonitor<Setting> intsettings,
        IOptionsMonitor<HLSSettings> inthlssettings,
        IOptionsMonitor<SDSettings> intsdsettings
    ) : IRequestHandler<GetSettingsRequest, APIResponse<SettingDto>>
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task<APIResponse<SettingDto>> Handle(GetSettingsRequest request, CancellationToken cancellationToken)
    {
        SettingDto ret = mapper.Map<SettingDto>(settings);

        ret.HLS = inthlssettings.CurrentValue;
        ret.SDSettings = intsdsettings.CurrentValue;

        return await Task.FromResult(APIResponse<SettingDto>.Success(ret));
    }
}