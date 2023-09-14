using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFilesQuery(M3UFileParameters Parameters) : IRequest<PagedResponse<M3UFileDto>>;

internal class GetM3UFilesQueryHandler : IRequestHandler<GetM3UFilesQuery, PagedResponse<M3UFileDto>>
{
    private IRepositoryWrapper Repository { get; }

    public GetM3UFilesQueryHandler(IRepositoryWrapper repository)
    {
        Repository = repository;
    }

    public async Task<PagedResponse<M3UFileDto>> Handle(GetM3UFilesQuery request, CancellationToken cancellationToken = default)
    {
        if (request.Parameters.PageSize == 0)
        {
            return request.Parameters.CreateEmptyPagedResponse<M3UFileDto>();
        }

        PagedResponse<M3UFileDto> m3uFiles = await Repository.M3UFile.GetPagedM3UFiles(request.Parameters).ConfigureAwait(false);
        return m3uFiles;
    }
}