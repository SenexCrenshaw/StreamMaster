using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineup(string lineup) : IRequest<LineUpResult?>;

internal class GetLineupHandler(ISDService sdService) : IRequestHandler<GetLineup, LineUpResult?>
{
    public async Task<LineUpResult?> Handle(GetLineup request, CancellationToken cancellationToken)
    {
        //Setting setting = await settingsService.GetSettingsAsync(cancellationToken);
        //SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        //SDStatus status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        //if (status == null || !status.systemStatus.Any())
        //{
        //    Console.WriteLine("Status is null");
        //    return null;
        //}

        //SDSystemStatus systemStatus = status.systemStatus[0];
        //if (systemStatus.status == "Offline")
        //{
        //    Console.WriteLine($"Status is {systemStatus.status}");
        //    return null;
        //}

        LineUpResult? ret = await sdService.GetLineup(request.lineup, cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
