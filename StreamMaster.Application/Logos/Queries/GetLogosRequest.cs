namespace StreamMaster.Application.Logos.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetLogosRequest() : IRequest<DataResponse<List<LogoFileDto>>>;

internal class GetLogosRequestHandler(ILogoService logoService)
    : IRequestHandler<GetLogosRequest, DataResponse<List<LogoFileDto>>>
{
    public Task<DataResponse<List<LogoFileDto>>> Handle(GetLogosRequest request, CancellationToken cancellationToken)
    {
        List<LogoFileDto> icons = logoService.GetLogos();

        return Task.FromResult(DataResponse<List<LogoFileDto>>.Success(icons));
    }
}