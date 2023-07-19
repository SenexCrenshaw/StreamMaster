using MediatR;

using StreamMaster.SchedulesDirectAPI;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineup(string lineup) : IRequest<LineUpResult?>;

internal class GetLineupHandler : IRequestHandler<GetLineup, LineUpResult?>
{
    public async Task<LineUpResult?> Handle(GetLineup request, CancellationToken cancellationToken)
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

        var ret = await sd.GetLineup(request.lineup, cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
