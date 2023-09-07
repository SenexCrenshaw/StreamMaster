using StreamMasterApplication.Services;

namespace StreamMasterApplication.M3UFiles.EventHandlers;

public class M3UFileProcessedEventHandler : BaseMediatorRequestHandler, INotificationHandler<M3UFileProcessedEvent>
{
    private readonly IBackgroundTaskQueue _taskQueue;

    public M3UFileProcessedEventHandler(IBackgroundTaskQueue taskQueue, ILogger<M3UFileProcessedEventHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext)
        : base(logger, repository, mapper, publisher, sender, hubContext) { _taskQueue = taskQueue; }


    public async Task Handle(M3UFileProcessedEvent notification, CancellationToken cancellationToken)
    {
        await _taskQueue.BuildIconsCacheFromVideoStreams(cancellationToken).ConfigureAwait(false);
        await HubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
        await HubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}