using StreamMaster.SchedulesDirectAPI.Helpers;
using System.Net;

using static System.Net.Mime.MediaTypeNames;

namespace StreamMasterApplication.SchedulesDirectAPI.Commands;

public record DownloadStationLogos() : IRequest;

public class DownloadStationLogosHandler(ISchedulesDirect schedulesDirect, ILogger<DownloadStationLogos> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<DownloadStationLogos>
{
    public async Task Handle(DownloadStationLogos request, CancellationToken cancellationToken)
    {
        
        
    }
    

}