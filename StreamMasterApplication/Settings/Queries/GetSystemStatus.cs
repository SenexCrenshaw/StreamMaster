using MediatR;

using Microsoft.Extensions.Caching.Memory;
using StreamMasterDomain.Cache;

namespace StreamMasterApplication.Settings.Queries;

public record GetSystemStatus : IRequest<SystemStatus>;

internal class GetSystemStatusHandler : IRequestHandler<GetSystemStatus, SystemStatus>
{
    private readonly IMemoryCache _memoryCache;

    public GetSystemStatusHandler(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<SystemStatus> Handle(GetSystemStatus request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SystemStatus { IsSystemReady = _memoryCache.IsSystemReady() });
    }
}
