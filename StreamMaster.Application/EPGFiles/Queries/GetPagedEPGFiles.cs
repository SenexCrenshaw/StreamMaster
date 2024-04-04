namespace StreamMaster.Application.EPGFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedEPGFiles(QueryStringParameters Parameters) : IRequest<PagedResponse<EPGFileDto>>;

public class GetPagedEPGFilesHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetPagedEPGFiles, PagedResponse<EPGFileDto>>
{
    public async Task<PagedResponse<EPGFileDto>> Handle(GetPagedEPGFiles request, CancellationToken cancellationToken = default)
    {
        PagedResponse<EPGFileDto> epgFiles = await Repository.EPGFile.GetPagedEPGFiles(request.Parameters);

        return request.Parameters.PageSize == 0 ? Repository.EPGFile.CreateEmptyPagedResponse() : epgFiles;
    }
}