using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetChannelNames : IRequest<List<StationChannelName>>;

internal class GetChannelNamesHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetChannelNames, List<StationChannelName>>
{

    public  Task<List<StationChannelName>> Handle(GetChannelNames request, CancellationToken cancellationToken)
    {
        var channelNames =  schedulesDirect.GetStationChannelNames();

        return Task.FromResult( channelNames);
    }
}
