using MediatR;

using StreamMasterInfrastructure.Pagination;

namespace StreamMasterApplication.M3UFiles.Queries;

public record GetM3UFilesQuery(M3UFileParameters Parameters) : IRequest<PagedList<M3UFile>>;

internal class GetM3UFilesQueryHandler : IRequestHandler<GetM3UFilesQuery, PagedList<M3UFile>>
{
    private IRepositoryWrapper Repository { get; }
    public GetM3UFilesQueryHandler(IRepositoryWrapper repository)
    {
        Repository = repository;
    }

    public async Task<PagedList<M3UFile>> Handle(GetM3UFilesQuery request, CancellationToken cancellationToken = default)
    {
        var m3uFiles = await Repository.M3UFile.GetM3UFilesAsync(request.Parameters).ConfigureAwait(false);
        return m3uFiles;
    }
}
