using MediatR;

using StreamMaster.SchedulesDirectAPI;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineups : IRequest<LineUpsResult?>;

internal class GetLineupsHandler : IRequestHandler<GetLineups, LineUpsResult?>
{
    public async Task<LineUpsResult?> Handle(GetLineups request, CancellationToken cancellationToken)
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

        var ret = await sd.GetLineups(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
