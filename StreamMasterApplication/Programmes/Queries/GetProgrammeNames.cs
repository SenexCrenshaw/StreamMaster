using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeNames : IRequest<List<string>>;

internal class GetProgrammeNamesHandler : BaseMemoryRequestHandler, IRequestHandler<GetProgrammeNames, List<string>>
{

    public GetProgrammeNamesHandler(ILogger<GetProgrammeNamesHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public Task<List<string>> Handle(GetProgrammeNames request, CancellationToken cancellationToken)
    {
        List<string> programmes = MemoryCache.Programmes()
            .Where(a => !string.IsNullOrEmpty(a.Channel) && a.StopDateTime > DateTime.Now.AddDays(-1))
            .Select(a => a.DisplayName).Distinct().ToList();

        return Task.FromResult(programmes);
    }
}
