namespace StreamMasterApplication.Programmes.EventHandlers;

public class ProgrammesProcessedEventHandler : BaseMediatorRequestHandler, INotificationHandler<ProgrammesProcessedEvent>
{
    public ProgrammesProcessedEventHandler(ILogger<ProgrammesProcessedEventHandler> logger, IRepositoryWrapper repository, IMapper mapper,ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache) { }


    public async Task Handle(ProgrammesProcessedEvent notification, CancellationToken cancellationToken)
    {
        await HubContext.Clients.All.ProgrammesRefresh().ConfigureAwait(false);
    }
}