namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetStationChannelNameFromDisplayName(string value) : IRequest<StationChannelName?>;

internal class GetStationChannelNameFromDisplayNameHandler(ILogger<GetStationChannelNameFromDisplayName> logger, ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetStationChannelNameFromDisplayName, StationChannelName?>
{
    public Task<StationChannelName?> Handle(GetStationChannelNameFromDisplayName request, CancellationToken cancellationToken)
    {
        IEnumerable<StationChannelName>? stationChannelNames = schedulesDirectDataService.GetStationChannelNames();
        if (stationChannelNames == null)
        {
            return Task.FromResult<StationChannelName?>(null);
        }
        StationChannelName? check = stationChannelNames.FirstOrDefault(a => a.Channel == request.value);
        return Task.FromResult(check);
    }
}
