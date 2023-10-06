using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStations : IRequest<List<Station>>;

internal class GetStationsHandler(ISettingsService settingsService) : IRequestHandler<GetStations, List<Station>>
{
    public async Task<List<Station>> Handle(GetStations request, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync();
        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);

        List<Station> ret = await sd.GetStations(cancellationToken).ConfigureAwait(false);


        return ret;
    }
}
