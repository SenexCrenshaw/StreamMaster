namespace StreamMaster.Application.M3UFiles.EventHandlers;

public class M3UFileProcessedEventHandler(ILogger<M3UFileProcessedEventHandler> logger, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext) : INotificationHandler<M3UFileProcessedEvent>
{
    public async Task Handle(M3UFileProcessedEvent notification, CancellationToken cancellationToken)
    {
        //await _taskQueue.BuildIconsCacheFromVideoStreams(cancellationToken).ConfigureAwait(false);
        await hubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
        await hubContext.Clients.All.ChannelGroupsRefresh().ConfigureAwait(false);
        await hubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}