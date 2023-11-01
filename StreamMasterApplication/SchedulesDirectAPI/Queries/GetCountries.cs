using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetCountries : IRequest<Countries?>;

internal class GetCountriesHandler(ISettingsService settingsService) : IRequestHandler<GetCountries, Countries?>
{
    public async Task<Countries?> Handle(GetCountries request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        SDStatus status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        if (status == null || !status.systemStatus.Any())
        {
            Console.WriteLine("Status is null");
            return null;
        }

        SDSystemStatus systemStatus = status.systemStatus[0];
        if (systemStatus.status == "Offline")
        {
            Console.WriteLine($"Status is {systemStatus.status}");
            return null;
        }

        Countries? ret = await sd.GetCountries(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
