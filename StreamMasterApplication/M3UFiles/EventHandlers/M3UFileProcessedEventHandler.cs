using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;
using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.Services;

namespace StreamMasterApplication.M3UFiles.EventHandlers;

public class M3UFileProcessedEventHandler : BaseMemoryRequestHandler, INotificationHandler<M3UFileProcessedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly IBackgroundTaskQueue _taskQueue;

    public M3UFileProcessedEventHandler(
        IBackgroundTaskQueue taskQueue,
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ILogger<ProcessM3UFileRequestHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache)
    {
        _hubContext = hubContext;
        _taskQueue = taskQueue;
    }

    public async Task Handle(M3UFileProcessedEvent notification, CancellationToken cancellationToken)
    {
        await _taskQueue.BuildIconsCacheFromVideoStreams(cancellationToken).ConfigureAwait(false);
        await _hubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
        await _hubContext.Clients.All.VideoStreamsRefresh().ConfigureAwait(false);
    }
}