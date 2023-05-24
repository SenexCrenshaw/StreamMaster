using MediatR;

using Microsoft.Extensions.Caching.Memory;

namespace StreamMasterApplication.Programmes.Queries;

public record GetProgrammeChannels : IRequest<List<ProgrammeChannel>>;

internal class GetProgrammeChannelsHandler : IRequestHandler<GetProgrammeChannels, List<ProgrammeChannel>>
{
    private readonly IMemoryCache _memoryCache;

    public GetProgrammeChannelsHandler(
        IMemoryCache memoryCache
        )
    {
        _memoryCache = memoryCache;
    }

    public Task<List<ProgrammeChannel>> Handle(GetProgrammeChannels request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_memoryCache.ProgrammeChannels());
    }
}
