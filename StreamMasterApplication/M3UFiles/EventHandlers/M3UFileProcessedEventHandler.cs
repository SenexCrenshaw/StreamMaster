using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.Services;
using StreamMasterApplication.VideoStreams.Queries;

namespace StreamMasterApplication.M3UFiles.EventHandlers;

public class M3UFileProcessedEventHandler : INotificationHandler<M3UFileProcessedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ISender _sender;
    private readonly IBackgroundTaskQueue _taskQueue;

    public M3UFileProcessedEventHandler(
        IBackgroundTaskQueue taskQueue,
        ISender sender,
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _taskQueue = taskQueue;
        _sender = sender;
        _hubContext = hubContext;
    }

    public async Task Handle(M3UFileProcessedEvent notification, CancellationToken cancellationToken)
    {
        var streams = await _sender.Send(new GetVideoStreams(), cancellationToken).ConfigureAwait(false);

        await _taskQueue.CacheIconsFromVideoStreams(cancellationToken).ConfigureAwait(false);
        await _hubContext.Clients.All.M3UFilesDtoUpdate(notification.M3UFile).ConfigureAwait(false);
        await _hubContext.Clients.All.VideoStreamDtoesUpdate(streams).ConfigureAwait(false);
    }
}
