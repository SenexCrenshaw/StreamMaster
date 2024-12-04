namespace StreamMaster.Application.SchedulesDirect.Queries;

public record GetChannelNamesRequest : IRequest<DataResponse<IEnumerable<string>>>;

internal class GetChannelNamesRequestHandler(ISchedulesDirectDataService schedulesDirectDataService, ICacheManager cacheManager)
    : IRequestHandler<GetChannelNamesRequest, DataResponse<IEnumerable<string>>>
{
    public Task<DataResponse<IEnumerable<string>>> Handle(GetChannelNamesRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<StationChannelName> channelNames = schedulesDirectDataService.GetStationChannelNames();
        System.Collections.Concurrent.ConcurrentDictionary<int, List<StationChannelName>> test = cacheManager.StationChannelNames;
        return Task.FromResult(DataResponse<IEnumerable<string>>.Success(channelNames.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase).Select(a => a.ChannelName)));
    }
}
