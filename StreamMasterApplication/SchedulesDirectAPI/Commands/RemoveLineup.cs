using StreamMaster.SchedulesDirectAPI.Domain.Commands;
using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

namespace StreamMasterApplication.SchedulesDirectAPI.Commands;

public record RemoveLineup(string lineup) : IRequest<bool>;

public class RemoveLineupHandler(ISDService sdService, ILogger<RemoveLineup> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<RemoveLineup, bool>
{
    public async Task<bool> Handle(RemoveLineup request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync().ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return false;
        }
        logger.LogInformation("Remove line up {lineup}", request.lineup);
        if (await sdService.RemoveLineup(request.lineup, cancellationToken).ConfigureAwait(false))
        {
            sdService.ResetCache(SDCommands.Status);
            sdService.ResetCache(SDCommands.LineUps);
            await hubContext.Clients.All.SchedulesDirectsRefresh();
            return true;
        }
        return false;
    }
}