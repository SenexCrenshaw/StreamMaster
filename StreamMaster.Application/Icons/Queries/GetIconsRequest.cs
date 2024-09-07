namespace StreamMaster.Application.Icons.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetIconsRequest() : IRequest<DataResponse<List<LogoFileDto>>>;

internal class GetIconsRequestHandler(ILogoService logoService)
    : IRequestHandler<GetIconsRequest, DataResponse<List<LogoFileDto>>>
{
    public Task<DataResponse<List<LogoFileDto>>> Handle(GetIconsRequest request, CancellationToken cancellationToken)
    {
        List<LogoFileDto> icons = logoService.GetLogos();

        return Task.FromResult(DataResponse<List<LogoFileDto>>.Success(icons));
    }
}