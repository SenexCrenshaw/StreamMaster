using StreamMasterApplication.Services;

namespace StreamMasterApplication.EPGFiles.EventHandlers;

public class EPGFileProcessedEventHandler : INotificationHandler<EPGFileProcessedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly IBackgroundTaskQueue _taskQueue;

    public EPGFileProcessedEventHandler(
         IBackgroundTaskQueue taskQueue,
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _taskQueue = taskQueue;
        _hubContext = hubContext;
    }

    public async Task Handle(EPGFileProcessedEvent notification, CancellationToken cancellationToken)
    {
        await _taskQueue.BuildProgIconsCacheFromEPGs(cancellationToken).ConfigureAwait(false);
        await _hubContext.Clients.All.EPGFilesRefresh().ConfigureAwait(false);
        await _hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
    }
}
