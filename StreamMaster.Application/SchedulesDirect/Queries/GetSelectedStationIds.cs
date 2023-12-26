using StreamMaster.Domain.Cache;

namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetSelectedStationIds : IRequest<List<StationIdLineup>>;

internal class GetSelectedStationIdsHandler(IMemoryCache memoryCache) : IRequestHandler<GetSelectedStationIds, List<StationIdLineup>>
{

    public Task<List<StationIdLineup>> Handle(GetSelectedStationIds request, CancellationToken cancellationToken)
    {
        var settings = memoryCache.GetSetting();

        return Task.FromResult(settings.SDSettings.SDStationIds.OrderBy(a => a.StationId, StringComparer.OrdinalIgnoreCase).ToList());
    }
}
