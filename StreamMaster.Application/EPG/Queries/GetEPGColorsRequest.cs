namespace StreamMaster.Application.EPG.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGColorsRequest() : IRequest<DataResponse<List<EPGColorDto>>>;

internal class GetEPGColorsHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGColorsRequest, DataResponse<List<EPGColorDto>>>
{
    public Task<DataResponse<List<EPGColorDto>>> Handle(GetEPGColorsRequest request, CancellationToken cancellationToken = default)
    {
        List<EPGColorDto> epgColors = Repository.EPGFile.GetEPGColors();
        return Task.FromResult(DataResponse<List<EPGColorDto>>.Success(epgColors));
    }
}
