namespace StreamMasterApplication.SchedulesDirect.Queries;

public record GetChannelNames : IRequest<List<string>>;

internal class GetChannelNamesHandler(ISchedulesDirectDataService schedulesDirectDataService) : IRequestHandler<GetChannelNames, List<string>>
{

    public Task<List<string>> Handle(GetChannelNames request, CancellationToken cancellationToken)
    {
        List<StationChannelName> channelNames = schedulesDirectDataService.GetStationChannelNames();

        return Task.FromResult(channelNames.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase).Select(a => a.ChannelName).ToList());
    }
}
