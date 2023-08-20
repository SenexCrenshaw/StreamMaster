using AutoMapper;

using MediatR;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeFromDisplayName(string Tvg_ID) : IRequest<ProgrammeNameDto?>;

internal class GetProgrammeFromDisplayNameHandler : BaseMemoryRequestHandler, IRequestHandler<GetProgrammeFromDisplayName, ProgrammeNameDto?>
{

    public GetProgrammeFromDisplayNameHandler(ILogger<GetProgrammeFromDisplayName> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache) { }

    public Task<ProgrammeNameDto?> Handle(GetProgrammeFromDisplayName request, CancellationToken cancellationToken)
    {
        ProgrammeNameDto? test = MemoryCache.GetEPGChannelByDisplayName(request.Tvg_ID);

        return Task.FromResult(test);
    }
}
