using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;
public record GetSchedules() : IRequest<List<Schedule>>;

public class GetSchedulesHandler(ILogger<GetSchedules> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<GetSchedules, List<Schedule>>
{
    public async Task<List<Schedule>> Handle(GetSchedules request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync();
        SchedulesDirect sd = new(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
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

        List<Schedule>? ret = await sd.GetSchedules(setting.SDStationIds.Select(a => a.StationId).ToList(), cancellationToken).ConfigureAwait(false);

        return ret ?? new();
    }
}
