namespace StreamMaster.Application.M3UFiles.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetPagedM3UFilesRequest(QueryStringParameters Parameters) : IRequest<PagedResponse<M3UFileDto>>;

internal class GetPagedM3UFilesRequestHandler(IRepositoryWrapper Repository)
    : IRequestHandler<GetPagedM3UFilesRequest, PagedResponse<M3UFileDto>>
{
    public async Task<PagedResponse<M3UFileDto>> Handle(GetPagedM3UFilesRequest request, CancellationToken cancellationToken)
    {
        if (request?.Parameters?.PageSize == null || request.Parameters.PageSize == 0)
        {
            return Repository.M3UFile.CreateEmptyPagedResponse();
        }

        PagedResponse<M3UFileDto> m3uFiles = await Repository.M3UFile.GetPagedM3UFiles(request.Parameters).ConfigureAwait(false);
        return m3uFiles;
    }
}
