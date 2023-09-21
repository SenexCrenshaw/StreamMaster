using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineup(string lineup) : IRequest<LineUpResult?>;

internal class GetLineupHandler(ISettingsService settingsService) : IRequestHandler<GetLineup, LineUpResult?>
{
    public async Task<LineUpResult?> Handle(GetLineup request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDCountry, setting.SDPassword);
        SDStatus? status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        if (status == null || !status.systemStatus.Any())
        {
            Console.WriteLine("Status is null");
            return null;
        }

        SDSystemstatus systemStatus = status.systemStatus[0];
        if (systemStatus.status == "Offline")
        {
            Console.WriteLine($"Status is {systemStatus.status}");
            return null;
        }

        LineUpResult? ret = await sd.GetLineup(request.lineup, cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
