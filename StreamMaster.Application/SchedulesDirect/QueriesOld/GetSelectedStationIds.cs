namespace StreamMaster.Application.SchedulesDirect.QueriesOld;

public record GetSelectedStationIds : IRequest<List<StationIdLineup>>;

internal class GetSelectedStationIdsHandler(IOptionsMonitor<SDSettings> intsettings) : IRequestHandler<GetSelectedStationIds, List<StationIdLineup>>
{
    private readonly SDSettings settings = intsettings.CurrentValue;
    public Task<List<StationIdLineup>> Handle(GetSelectedStationIds request, CancellationToken cancellationToken)
    {

        return Task.FromResult(settings.SDStationIds.OrderBy(a => a.StationId, StringComparer.OrdinalIgnoreCase).ToList());
    }
}
