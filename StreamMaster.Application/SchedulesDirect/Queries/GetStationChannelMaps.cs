namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetStationChannelMaps : IRequest<List<StationChannelMap>>;

internal class GetStationChannelMapsHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetStationChannelMaps, List<StationChannelMap>>
{

    public async Task<List<StationChannelMap>> Handle(GetStationChannelMaps request, CancellationToken cancellationToken)
    {
        var sm = await schedulesDirect.GetStationChannelMaps(cancellationToken);

        return sm ?? [];
    }
}
