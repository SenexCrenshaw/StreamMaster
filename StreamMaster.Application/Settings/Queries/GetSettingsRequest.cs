namespace StreamMaster.Application.Settings.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetSettingsRequest : IRequest<DataResponse<SettingDto>>;

internal class GetSettingsRequestHandler(
        IMapper mapper,
        IOptionsMonitor<Setting> intSettings,
        IOptionsMonitor<SDSettings> intSDSettings

    ) : IRequestHandler<GetSettingsRequest, DataResponse<SettingDto>>
{
    private readonly Setting settings = intSettings.CurrentValue;

    public async Task<DataResponse<SettingDto>> Handle(GetSettingsRequest request, CancellationToken cancellationToken)
    {
        SettingDto ret = mapper.Map<SettingDto>(settings);

        ret.SDSettings = intSDSettings.CurrentValue;

        return await Task.FromResult(DataResponse<SettingDto>.Success(ret));
    }
}