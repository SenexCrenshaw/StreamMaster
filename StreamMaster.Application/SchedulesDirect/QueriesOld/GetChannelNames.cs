namespace StreamMaster.Application.SchedulesDirect.QueriesOld;

public record GetChannelNames : IRequest<List<string>>;

internal class GetChannelNamesHandler(ISchedulesDirectDataService schedulesDirectDataService) : IRequestHandler<GetChannelNames, List<string>>
{

    public async Task<List<string>> Handle(GetChannelNames request, CancellationToken cancellationToken)
    {
        List<StationChannelName> channelNames = await schedulesDirectDataService.GetStationChannelNames();

        return channelNames.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase).Select(a => a.ChannelName).ToList();
    }
}
