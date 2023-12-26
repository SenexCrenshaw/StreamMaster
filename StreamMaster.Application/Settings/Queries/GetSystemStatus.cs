using StreamMaster.Domain.Cache;

namespace StreamMaster.Application.Settings.Queries;

public record GetSystemStatus : IRequest<SDSystemStatus>;

internal class GetSystemStatusHandler : IRequestHandler<GetSystemStatus, SDSystemStatus>
{
    private readonly IMemoryCache _memoryCache;

    public GetSystemStatusHandler(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<SDSystemStatus> Handle(GetSystemStatus request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SDSystemStatus { IsSystemReady = _memoryCache.IsSystemReady() });
    }
}
