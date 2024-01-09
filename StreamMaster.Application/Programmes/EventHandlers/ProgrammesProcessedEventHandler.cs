namespace StreamMaster.Application.Programmes.EventHandlers;

public class ProgrammesProcessedEventHandler(ILogger<ProgrammesProcessedEventHandler> logger, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : INotificationHandler<ProgrammesProcessedEvent>
{
    public async Task Handle(ProgrammesProcessedEvent notification, CancellationToken cancellationToken)
    {
        await HubContext.Clients.All.ProgrammesRefresh().ConfigureAwait(false);
    }
}