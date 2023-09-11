using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;
public record GetSchedules() : IRequest<List<Schedule>>;

public class GetSchedulesHandler(ILogger<GetSchedules> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext), IRequestHandler<GetSchedules, List<Schedule>>
{
    public async Task<List<Schedule>> Handle(GetSchedules request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync();
        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDCountry, setting.SDPassword);
        bool status = await sd.GetSystemReady(cancellationToken).ConfigureAwait(false);
        if (!status)
        {
            Console.WriteLine($"Status is {status}");
            return new();
        }

        if (setting.SDStationIds == null || !setting.SDStationIds.Any())
        {
            Console.WriteLine($"No station ids");
            return new();
        }

        List<Schedule>? ret = await sd.GetSchedules(setting.SDStationIds, cancellationToken).ConfigureAwait(false);

        return ret ?? new();
    }
}
