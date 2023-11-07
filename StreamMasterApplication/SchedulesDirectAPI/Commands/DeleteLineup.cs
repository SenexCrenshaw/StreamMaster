using StreamMaster.SchedulesDirectAPI.Domain.Commands;
using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

namespace StreamMasterApplication.SchedulesDirectAPI.Commands;

public record DeleteLineup(string lineup) : IRequest<bool>;

public class DeleteLineupHandler(ISDService sdService, ILogger<DeleteLineup> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<DeleteLineup, bool>
{
    public async Task<bool> Handle(DeleteLineup request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync().ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return false;
        }
        logger.LogInformation("Delete line up {lineup}", request.lineup);
        if (await sdService.DeleteLineup(request.lineup, cancellationToken).ConfigureAwait(false))
        {
            sdService.ResetCache(SDCommands.Status);
            await hubContext.Clients.All.SchedulesDirectsRefresh();
            return true;
        }
        return false;
    }
}