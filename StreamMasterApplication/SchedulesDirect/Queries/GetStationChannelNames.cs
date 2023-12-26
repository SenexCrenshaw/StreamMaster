namespace StreamMasterApplication.SchedulesDirect.Queries;

public record GetStationChannelNames : IRequest<List<StationChannelName>>;

internal class GetStationChannelNamesHandler(ISchedulesDirectDataService schedulesDirectDataService) : IRequestHandler<GetStationChannelNames, List<StationChannelName>>
{

    public Task<List<StationChannelName>> Handle(GetStationChannelNames request, CancellationToken cancellationToken)
    {
        List<StationChannelName> channelNames = schedulesDirectDataService.GetStationChannelNames();

        return Task.FromResult(channelNames);
    }
}
