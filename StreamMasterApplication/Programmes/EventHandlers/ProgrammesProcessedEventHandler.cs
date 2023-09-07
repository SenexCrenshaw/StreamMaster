namespace StreamMasterApplication.Programmes.EventHandlers;

public class ProgrammesProcessedEventHandler : BaseMediatorRequestHandler, INotificationHandler<ProgrammesProcessedEvent>
{
    public ProgrammesProcessedEventHandler(ILogger<ProgrammesProcessedEventHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
: base(logger, repository, mapper, publisher, sender, hubContext) { }


    public async Task Handle(ProgrammesProcessedEvent notification, CancellationToken cancellationToken)
    {
        await HubContext.Clients.All.ProgrammesRefresh().ConfigureAwait(false);
    }
}