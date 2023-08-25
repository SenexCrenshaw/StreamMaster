using MediatR;

using StreamMasterDomain.Dto;
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
        int count = Repository.M3UFile.Count();

        if (request.Parameters.PageSize == 0)
        {
            PagedResponse<M3UFileDto> emptyResponse = new();
            emptyResponse.TotalItemCount = count;
            return emptyResponse;
        }

        PagedResponse<M3UFileDto> m3uFiles = await Repository.M3UFile.GetM3UFilesAsync(request.Parameters).ConfigureAwait(false);
        return m3uFiles;
    }
}