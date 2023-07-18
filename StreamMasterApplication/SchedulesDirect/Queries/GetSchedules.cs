using MediatR;

using StreamMaster.SchedulesDirect;

namespace StreamMasterApplication.Settings.Queries;

public record GetSchedules(List<string> stationIds) : IRequest<List<Schedule>?>;

internal class GetSchedulesHandler : IRequestHandler<GetSchedules, List<Schedule>?>
{
    public async Task<List<Schedule>?> Handle(GetSchedules request, CancellationToken cancellationToken)
    {
        Setting setting = FileUtil.GetSetting();

        var sd = new SchedulesDirect(setting.SDUserName, setting.SDPassword);
        var status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        if (status == null || !status.systemStatus.Any())
        {
            Console.WriteLine("Status is null");
            return null;
        }

        var systemStatus = status.systemStatus[0];
        if (systemStatus.status == "Offline")
        {
            Console.WriteLine($"Status is {systemStatus.status}");
            return null;
        }

        var ret = await sd.GetSchedules(request.stationIds, cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
