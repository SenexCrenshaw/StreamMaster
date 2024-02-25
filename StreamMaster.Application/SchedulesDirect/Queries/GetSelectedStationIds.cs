using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetSelectedStationIds : IRequest<List<StationIdLineup>>;

internal class GetSelectedStationIdsHandler(IOptionsMonitor<Setting> intsettings) : IRequestHandler<GetSelectedStationIds, List<StationIdLineup>>
{
    private readonly Setting settings = intsettings.CurrentValue;
    public Task<List<StationIdLineup>> Handle(GetSelectedStationIds request, CancellationToken cancellationToken)
    {

        return Task.FromResult(settings.SDSettings.SDStationIds.OrderBy(a => a.StationId, StringComparer.OrdinalIgnoreCase).ToList());
    }
}
