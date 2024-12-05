namespace StreamMaster.Application.Logos.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetCustomLogosRequest() : IRequest<DataResponse<List<CustomLogoDto>>>;

internal class GetCustomLogosRequestHandler(IOptionsMonitor<CustomLogoDict> customLogos)
    : IRequestHandler<GetCustomLogosRequest, DataResponse<List<CustomLogoDto>>>
{
    public Task<DataResponse<List<CustomLogoDto>>> Handle(GetCustomLogosRequest request, CancellationToken cancellationToken)
    {
        List<CustomLogoDto> icons = [.. customLogos.CurrentValue.GetCustomLogosDto().OrderBy(a => a.Name)];

        return Task.FromResult(DataResponse<List<CustomLogoDto>>.Success(icons));
    }
}