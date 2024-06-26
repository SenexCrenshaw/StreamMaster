namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGFilePreviewByIdRequest(int Id) : IRequest<DataResponse<List<EPGFilePreviewDto>>>;

internal class GetEPGFilePreviewByIdHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGFilePreviewByIdRequest, DataResponse<List<EPGFilePreviewDto>>>
{
    public async Task<DataResponse<List<EPGFilePreviewDto>>> Handle(GetEPGFilePreviewByIdRequest request, CancellationToken cancellationToken = default)
    {
        List<EPGFilePreviewDto> res = await Repository.EPGFile.GetEPGFilePreviewById(request.Id, cancellationToken).ConfigureAwait(false);

        return DataResponse<List<EPGFilePreviewDto>>.Success(res);
    }
}
