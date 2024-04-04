using StreamMaster.Domain.Repository;

namespace StreamMaster.Application.M3UFiles.QueriesOld;

public record GetM3UFileNamesQuery() : IRequest<List<string>>;

internal class GetM3UFileNamesQueryHandler : IRequestHandler<GetM3UFileNamesQuery, List<string>>
{
    private IRepositoryWrapper Repository { get; }

    public GetM3UFileNamesQueryHandler(IRepositoryWrapper repository)
    {
        Repository = repository;
    }

    public async Task<List<string>> Handle(GetM3UFileNamesQuery request, CancellationToken cancellationToken = default)
    {
        return await Repository.M3UFile.GetM3UFileNames();
    }
}