namespace StreamMaster.Application.EPGFiles.EventHandlers;

//public class EPGFileDeletedEventHandler : INotificationHandler<EPGFileDeletedEvent>
//{
//    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;

//    public EPGFileDeletedEventHandler(
//  IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
//        )
//    {
//        _hubContext = hubContext;
//    }

//    public async Task Handle(EPGFileDeletedEvent notification, CancellationToken cancellationToken)
//    {
//        //await _hubContext.ClientChannels.All.EPGFilesRefresh().ConfigureAwait(false);
//        //await _hubContext.ClientChannels.All.ChannelGroupsRefresh().ConfigureAwait(false);
//        //await _hubContext.ClientChannels.All.SchedulesDirectsRefresh().ConfigureAwait(false);
//        //await _hubContext.ClientChannels.All.CacheHandler("epgSelector").ConfigureAwait(false);
//    }
//}
