using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetPagedM3UFiles(M3UFileParameters Parameters) : IRequest<PagedResponse<M3UFileDto>>;

internal class GetPagedM3UFilesHandler : IRequestHandler<GetPagedM3UFiles, PagedResponse<M3UFileDto>>
{
    private IRepositoryWrapper Repository { get; }

    public GetPagedM3UFilesHandler(IRepositoryWrapper repository)
    {
        Repository = repository;
    }

    public async Task<PagedResponse<M3UFileDto>> Handle(GetPagedM3UFiles request, CancellationToken cancellationToken = default)
    {
        if (request.Parameters.PageSize == 0)
        {
            return Repository.M3UFile.CreateEmptyPagedResponse();
        }

        PagedResponse<M3UFileDto> m3uFiles = await Repository.M3UFile.GetPagedM3UFiles(request.Parameters).ConfigureAwait(false);
        return m3uFiles;
    }
}