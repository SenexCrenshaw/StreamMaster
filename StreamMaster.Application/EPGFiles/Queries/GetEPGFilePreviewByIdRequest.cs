namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetEPGFilePreviewByIdRequest(int Id) : IRequest<APIResponse<List<EPGFilePreviewDto>>>;

internal class GetEPGFilePreviewByIdHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetEPGFilePreviewByIdRequest, APIResponse<List<EPGFilePreviewDto>>>
{
    public async Task<APIResponse<List<EPGFilePreviewDto>>> Handle(GetEPGFilePreviewByIdRequest request, CancellationToken cancellationToken = default)
    {
        List<EPGFilePreviewDto> res = await Repository.EPGFile.GetEPGFilePreviewById(request.Id, cancellationToken).ConfigureAwait(false);

        return APIResponse<List<EPGFilePreviewDto>>.Success(res);
    }
}
