using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Hubs;

using StreamMasterApplication.Services;

namespace StreamMasterApplication.Programmes.EventHandlers;

public class ProgrammesProcessedEventHandler : BaseMemoryRequestHandler, INotificationHandler<ProgrammesProcessedEvent>
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly IBackgroundTaskQueue _taskQueue;

    public ProgrammesProcessedEventHandler(
        IBackgroundTaskQueue taskQueue,
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ILogger<ProgrammesProcessedEventHandler> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IMemoryCache memoryCache)
        : base(logger, repository, mapper, publisher, sender, memoryCache)
    {
        _hubContext = hubContext;
        _taskQueue = taskQueue;
    }

    public async Task Handle(ProgrammesProcessedEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.All.ProgrammesRefresh().ConfigureAwait(false);
    }
}