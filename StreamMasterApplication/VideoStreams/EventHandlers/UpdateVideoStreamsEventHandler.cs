using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class UpdateVideoStreamsEventHandler : INotificationHandler<UpdateVideoStreamsEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ILogger<UpdateVideoStreamsEventHandler> _logger;

    public UpdateVideoStreamsEventHandler(
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        ILogger<UpdateVideoStreamsEventHandler> logger
        )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Handle(UpdateVideoStreamsEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Domain Event: {DomainEvent}", notification.GetType().Name);

        await _hubContext.Clients.All.VideoStreamDtoesUpdate(notification.VideoStreamDtos).ConfigureAwait(false);
    }
}