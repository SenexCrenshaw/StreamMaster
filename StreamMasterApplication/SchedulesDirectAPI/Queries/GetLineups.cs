using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineups : IRequest<LineUpsResult?>;

internal class GetLineupsHandler(ISettingsService settingsService) : IRequestHandler<GetLineups, LineUpsResult?>
{
    public async Task<LineUpsResult?> Handle(GetLineups request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        bool isReady = await sd.GetSystemReady(cancellationToken).ConfigureAwait(false);
        if (!isReady)
        {
            Console.WriteLine($"Status is Offline");
            return null;
        }

        LineUpsResult? ret = await sd.GetLineups(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
