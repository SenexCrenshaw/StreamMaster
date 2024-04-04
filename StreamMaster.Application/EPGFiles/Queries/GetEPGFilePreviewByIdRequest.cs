namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGFilePreviewByIdRequest(int Id) : IRequest<List<EPGFilePreviewDto>>;

internal class GetEPGFilePreviewByIdHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGFilePreviewByIdRequest, List<EPGFilePreviewDto>>
{
    public async Task<List<EPGFilePreviewDto>> Handle(GetEPGFilePreviewByIdRequest request, CancellationToken cancellationToken = default)
    {
        List<EPGFilePreviewDto> res = await Repository.EPGFile.GetEPGFilePreviewById(request.Id, cancellationToken).ConfigureAwait(false);

        return res;
    }
}
