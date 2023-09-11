using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStatus : IRequest<SDStatus>;

internal class GetStatusHandler(ISettingsService settingsService) : IRequestHandler<GetStatus, SDStatus>
{

    public async Task<SDStatus> Handle(GetStatus request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDCountry, setting.SDPassword);
        SDStatus? status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        return status ?? new SDStatus();
    }
}
