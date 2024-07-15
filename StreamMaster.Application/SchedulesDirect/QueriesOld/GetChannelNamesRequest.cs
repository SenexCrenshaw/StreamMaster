namespace StreamMaster.Application.SchedulesDirect.QueriesOld;


public record GetChannelNamesRequest : IRequest<DataResponse<IEnumerable<string>>>;

internal class GetChannelNamesRequestHandler(ISchedulesDirectDataService schedulesDirectDataService)
    : IRequestHandler<GetChannelNamesRequest, DataResponse<IEnumerable<string>>>
{
    public Task<DataResponse<IEnumerable<string>>> Handle(GetChannelNamesRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<StationChannelName> channelNames = schedulesDirectDataService.GetStationChannelNames();

        return Task.FromResult(DataResponse<IEnumerable<string>>.Success(channelNames.OrderBy(a => a.DisplayName, StringComparer.OrdinalIgnoreCase).Select(a => a.ChannelName)));
    }
}
