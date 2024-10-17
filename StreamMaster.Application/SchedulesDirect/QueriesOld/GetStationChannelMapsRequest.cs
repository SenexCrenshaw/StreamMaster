namespace StreamMaster.Application.SchedulesDirect.QueriesOld;


public record GetStationChannelMapsRequest : IRequest<DataResponse<List<StationChannelMap>>>;

internal class GetStationChannelMapsHandler(ILineupService lineups) : IRequestHandler<GetStationChannelMapsRequest, DataResponse<List<StationChannelMap>>>
{

    public async Task<DataResponse<List<StationChannelMap>>> Handle(GetStationChannelMapsRequest request, CancellationToken cancellationToken)
    {
        List<StationChannelMap> sm = await lineups.GetStationChannelMaps(cancellationToken);

        return DataResponse<List<StationChannelMap>>.Success(sm ?? []);
    }
}
