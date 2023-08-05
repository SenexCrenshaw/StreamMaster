using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class CreateVideoStreamEventHandler : INotificationHandler<CreateVideoStreamEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ILogger<CreateVideoStreamEventHandler> _logger;
    private readonly ISender _sender;

    public CreateVideoStreamEventHandler(
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
           ISender sender,
        ILogger<CreateVideoStreamEventHandler> logger
        )
    {
        _sender = sender;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Handle(CreateVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        await _hubContext.Clients.All.VideoStreamDtoUpdate(notification.VideoStreamsDto).ConfigureAwait(false);
    }
}
