using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Hubs;
using StreamMasterApplication.Services;

namespace StreamMasterAPI.Services;

public class PostStartup : BackgroundService
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ILogger _logger;
    private readonly ISender _mediator;
    private readonly IServiceProvider _serviceProvider;
    private readonly IBackgroundTaskQueue _taskQueue;

    public PostStartup(
        ILogger<PostStartup> logger,
        ISender mediator,
        IHubContext<StreamMasterHub, IStreamMasterHub> hubContext,
        IServiceProvider serviceProvider,
        IBackgroundTaskQueue taskQueue
        )
    {
        (_logger, _serviceProvider, _taskQueue, _mediator, _hubContext) =
        (logger, serviceProvider, taskQueue, mediator, hubContext);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        _logger.LogInformation(
        $"{nameof(PostStartup)} is running.");

        await _hubContext.Clients.All.SystemStatusUpdate(new StreamMasterApplication.Settings.Queries.SystemStatus { IsSystemReady = false }).ConfigureAwait(false);

        await _taskQueue.ScanDirectoryForIconFiles(cancellationToken).ConfigureAwait(false);

        await _taskQueue.ReadDirectoryLogosRequest(cancellationToken).ConfigureAwait(false);

        await _taskQueue.ScanDirectoryForEPGFiles(cancellationToken).ConfigureAwait(false);

        await _taskQueue.ScanDirectoryForM3UFiles(cancellationToken).ConfigureAwait(false);

        await _taskQueue.ProcessM3UFiles(cancellationToken).ConfigureAwait(false);

        await _taskQueue.SetIsSystemReady(true, cancellationToken).ConfigureAwait(false);

        await _taskQueue.CacheAllIcons(cancellationToken).ConfigureAwait(false);
    }
}
