using MediatR;

using Microsoft.EntityFrameworkCore;

namespace StreamMasterApplication.M3UFiles.Queries;

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
        return await Repository.M3UFile.GetM3UFileNames().ToListAsync();
    }
}