using StreamMaster.Application.Interfaces;

namespace StreamMaster.Application.Programmes.EventHandlers;

public class ProgrammesProcessedEventHandler()
    : INotificationHandler<ProgrammesProcessedEvent>
{
    public async Task Handle(ProgrammesProcessedEvent notification, CancellationToken cancellationToken)
    {
        //await HubContext.Clients.All.ProgrammesRefresh().ConfigureAwait(false);
    }
}