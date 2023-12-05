
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStationChannelNames : IRequest<List<StationChannelName>>;

internal class GetStationChannelNamesHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetStationChannelNames, List<StationChannelName>>
{

    public  Task<List<StationChannelName>> Handle(GetStationChannelNames request, CancellationToken cancellationToken)
    {
        var channelNames =  schedulesDirect.GetStationChannelNames();

        return Task.FromResult(channelNames);
    }
}
