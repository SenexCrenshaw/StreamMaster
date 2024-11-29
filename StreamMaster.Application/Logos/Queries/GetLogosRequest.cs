namespace StreamMaster.Application.Logos.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetLogosRequest() : IRequest<DataResponse<List<CustomLogoDto>>>;

internal class GetLogosRequestHandler(ILogoService logoService)
    : IRequestHandler<GetLogosRequest, DataResponse<List<CustomLogoDto>>>
{
    public Task<DataResponse<List<CustomLogoDto>>> Handle(GetLogosRequest request, CancellationToken cancellationToken)
    {
        List<CustomLogoDto> icons = logoService.GetLogos();

        return Task.FromResult(DataResponse<List<CustomLogoDto>>.Success(icons));
    }
}