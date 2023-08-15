using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.VideoStreams.Events;

namespace StreamMasterApplication.VideoStreams.EventHandlers;

public class UpdateVideoStreamEventHandler : INotificationHandler<UpdateVideoStreamEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ILogger<UpdateVideoStreamEventHandler> _logger;

    public UpdateVideoStreamEventHandler(IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ILogger<UpdateVideoStreamEventHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Handle(UpdateVideoStreamEvent notification, CancellationToken cancellationToken = default)
    {

        await _hubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}