using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetChannelNames : IRequest<List<string>>;

internal class GetChannelNamesHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetChannelNames, List<string>>
{

    public  Task<List<string>> Handle(GetChannelNames request, CancellationToken cancellationToken)
    {
        var channelNames =  schedulesDirect.GetStationChannelNames();

        return Task.FromResult( channelNames.OrderBy(a=>a.DisplayName, StringComparer.OrdinalIgnoreCase) .Select(a=> a.ChannelName).ToList());
    }
}
