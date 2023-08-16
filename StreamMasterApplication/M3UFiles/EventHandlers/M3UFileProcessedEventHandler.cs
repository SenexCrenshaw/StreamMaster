using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.Services;

namespace StreamMasterApplication.M3UFiles.EventHandlers;

public class M3UFileProcessedEventHandler : INotificationHandler<M3UFileProcessedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ISender _sender;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;

    public M3UFileProcessedEventHandler(
        IRepositoryWrapper repositoryWrapper,
        IBackgroundTaskQueue taskQueue,
        ISender sender,
         IMapper mapper,
         IHubContext<StreamMasterHub, IStreamMasterHub> hubContext
        )
    {
        _repositoryWrapper = repositoryWrapper;
        _taskQueue = taskQueue;
        _sender = sender;
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task Handle(M3UFileProcessedEvent notification, CancellationToken cancellationToken)
    {
        //var streams = _repositoryWrapper.VideoStream.GetVideoStreamsByM3UFileId(notification.M3UFile.Id);
        //var toSend = _mapper.Map<IEnumerable<VideoStreamDto>>(streams);
        //var streams = await _sender.Send(new GetVideoStreams(), cancellationToken).ConfigureAwait(false);

        await _taskQueue.BuildIconsCacheFromVideoStreams(cancellationToken).ConfigureAwait(false);
        await _hubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
        await _hubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}