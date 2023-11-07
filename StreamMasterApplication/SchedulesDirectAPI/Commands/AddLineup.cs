using StreamMaster.SchedulesDirectAPI.Domain.Commands;
using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

namespace StreamMasterApplication.SchedulesDirectAPI.Commands;

public record AddLineup(string lineup) : IRequest<bool>;

public class AddLineupHandler(ISDService sdService, ILogger<AddLineup> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<AddLineup, bool>
{
    public async Task<bool> Handle(AddLineup request, CancellationToken cancellationToken)
    {
        Setting setting = await GetSettingsAsync().ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return false;
        }
        logger.LogInformation("Add line up {lineup}", request.lineup);
        if (await sdService.AddLineup(request.lineup, cancellationToken).ConfigureAwait(false))
        {
            sdService.ResetCache(SDCommands.Status);
            await hubContext.Clients.All.SchedulesDirectsRefresh();
            return true;
        }
        return false;
    }
}