namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetStationChannelNameFromDisplayName(string value) : IRequest<StationChannelName?>;

internal class GetStationChannelNameFromDisplayNameHandler(ILogger<GetStationChannelNameFromDisplayName> logger, ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetStationChannelNameFromDisplayName, StationChannelName?>
{
    public async Task<StationChannelName?> Handle(GetStationChannelNameFromDisplayName request, CancellationToken cancellationToken)
    {
        List<StationChannelName> stationChannelNames = await schedulesDirectDataService.GetStationChannelNames();

        StationChannelName? check = stationChannelNames.FirstOrDefault(a => a.Channel == request.value);
        return check;
    }
}
