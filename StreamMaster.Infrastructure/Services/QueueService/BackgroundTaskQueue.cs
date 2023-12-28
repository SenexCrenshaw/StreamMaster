using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMaster.Application.Common.Interfaces;
using StreamMaster.Application.Common.Models;
using StreamMaster.Application.Hubs;
using StreamMaster.Application.Services;
using StreamMaster.Domain.Enums;

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Infrastructure.Services.QueueService;


public partial class BackgroundTaskQueue : IBackgroundTaskQueue
{
    //private static readonly object lockObject = new();
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ILogger<BackgroundTaskQueue> _logger;
    private readonly Channel<BackgroundTaskQueueConfig> _queue;
    private readonly ISender _sender;
    private readonly ConcurrentDictionary<Guid, TaskQueueStatus> taskQueueStatuses = new();

    public BackgroundTaskQueue(int capacity, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, ILogger<BackgroundTaskQueue> logger, ISender sender)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _sender = sender;
        _hubContext = hubContext;
        _queue = Channel.CreateBounded<BackgroundTaskQueueConfig>(options);
        _logger = logger;
    }

    public async ValueTask<BackgroundTaskQueueConfig> DeQueueAsync(CancellationToken cancellationToken)
    {
        BackgroundTaskQueueConfig workItem = await _queue.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Got {workItem.command} from Queue", workItem.Command);
        return workItem;
    }

    public Task<List<TaskQueueStatus>> GetQueueStatus()
    {

        return Task.FromResult(taskQueueStatuses.Values.OrderBy(a => a.StartTS).ToList());

    }

    public bool HasJobs()
    {
        return taskQueueStatuses.Values.Any(a => a.StopTS == DateTime.MinValue);
    }

    public async ValueTask SetIsSystemReady(bool isSystemReady, CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.SetIsSystemReady, isSystemReady, cancellationToken).ConfigureAwait(false);
    }

    public async Task SetQueueTS(Guid Id)
    {
        if (taskQueueStatuses.TryGetValue(Id, out TaskQueueStatus? status))
        {
            status.QueueTS = DateTime.Now;
            status.IsRunning = true;
            await _hubContext.Clients.All.TaskQueueStatusUpdate(await GetQueueStatus()).ConfigureAwait(false);
        }
    }

    public async Task SetStart(Guid Id)
    {
        if (taskQueueStatuses.TryGetValue(Id, out TaskQueueStatus? status))
        {
            status.StartTS = DateTime.Now;
            status.IsRunning = true;
            await _hubContext.Clients.All.TaskQueueStatusUpdate(await GetQueueStatus()).ConfigureAwait(false);
        }

    }

    public async Task SetStop(Guid Id)
    {
        if (taskQueueStatuses.TryGetValue(Id, out TaskQueueStatus? status))
        {
            status.StopTS = DateTime.Now;
            status.IsRunning = false;
            await _hubContext.Clients.All.TaskQueueStatusUpdate(await GetQueueStatus()).ConfigureAwait(false);
        }
    }

    private async ValueTask QueueAsync(SMQueCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Added {command} to Queue", command);
        BackgroundTaskQueueConfig bq = new() { Command = command, CancellationToken = cancellationToken };
        await QueueAsync(bq).ConfigureAwait(false);
    }

    private async ValueTask QueueAsync(SMQueCommand command, object entity, CancellationToken cancellationToken = default)
    {

        _logger.LogInformation("Added {command} to Queue", command);
        BackgroundTaskQueueConfig bq = new() { Command = command, Entity = entity, CancellationToken = cancellationToken };
        await QueueAsync(bq).ConfigureAwait(false);
    }

    private SMQueCommand lastSMQueCommand = new();
    private async ValueTask QueueAsync(BackgroundTaskQueueConfig workItem)
    {
        //No need to stack up the same task
        if (lastSMQueCommand == workItem.Command)
        {
            return;
        }

        lastSMQueCommand = workItem.Command;

        _ = taskQueueStatuses.TryAdd(workItem.Id, new TaskQueueStatus
        {
            Id = taskQueueStatuses.Count,
            Command = workItem.Command.ToString(),
        });

        await _hubContext.Clients.All.TaskQueueStatusUpdate(await GetQueueStatus()).ConfigureAwait(false);
        await _queue.Writer.WriteAsync(workItem).ConfigureAwait(false);
        _logger.LogInformation("Added {workItem.command} to Queue", workItem.Command);

    }
}
