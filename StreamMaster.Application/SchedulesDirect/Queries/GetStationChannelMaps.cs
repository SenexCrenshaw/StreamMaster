namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetStationChannelMaps : IRequest<List<StationChannelMap>>;

internal class GetStationChannelMapsHandler(ILineups lineups) : IRequestHandler<GetStationChannelMaps, List<StationChannelMap>>
{

    public async Task<List<StationChannelMap>> Handle(GetStationChannelMaps request, CancellationToken cancellationToken)
    {
        List<StationChannelMap> sm = await lineups.GetStationChannelMaps(cancellationToken);

        return sm ?? [];
    }
}
