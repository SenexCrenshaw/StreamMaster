using MediatR;

using Microsoft.Extensions.Logging;

using StreamMaster.Application.Common.Models;
using StreamMaster.Application.Services;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Infrastructure.Services.QueueService;

public partial class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly ILogger<BackgroundTaskQueue> _logger;
    private readonly Channel<BackgroundTaskQueueConfig> _queue;
    private readonly ISender _sender;
    private readonly ConcurrentDictionary<Guid, SMTask> taskQueueStatuses = new();
    private readonly IDataRefreshService dataRefreshService;
    public BackgroundTaskQueue(int capacity, ILogger<BackgroundTaskQueue> logger, ISender sender, IDataRefreshService dataRefreshService)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _sender = sender;
        _queue = Channel.CreateBounded<BackgroundTaskQueueConfig>(options);
        _logger = logger;
        this.dataRefreshService = dataRefreshService;
    }

    public async ValueTask<BackgroundTaskQueueConfig> DeQueueAsync(CancellationToken cancellationToken)
    {
        BackgroundTaskQueueConfig workItem = await _queue.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Got {workItem.command} from Queue", workItem.Command);
        return workItem;
    }

    public Task<List<SMTask>> GetQueueStatus()
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

    public async ValueTask SetTestTask(int DelayInSeconds, CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.SetTestTask, DelayInSeconds, cancellationToken).ConfigureAwait(false);
    }
    public List<SMTask> GetSMTasks()
    {
        return taskQueueStatuses.Values.OrderByDescending(a => a.Id).ToList();
    }

    private async ValueTask SendSMTasks()
    {
        var toSend = GetSMTasks();
        await dataRefreshService.RefreshSMTasks();// .SendSMTasks(toSend).ConfigureAwait(false);
    }



    //public async Task SetQueueTS(Guid Id)
    //{
    //    if (taskQueueStatuses.TryGetValue(Id, out SMTask? status))
    //    {
    //        status.QueueTS = SMDT.UtcNow;
    //        status.IsRunning = true;
    //        //await _hubContext.Clients.All.TaskQueueStatusUpdate(await GetQueueStatus()).ConfigureAwait(false);
    //        await SendSMTasks();
    //    }
    //}

    public async Task SetStart(Guid Id)
    {
        if (taskQueueStatuses.TryGetValue(Id, out SMTask? status))
        {
            if (status.Command == "SetIsSystemReady")
            {
                status.StartTS = BuildInfo.StartTime;
            }
            else
            {
                status.StartTS = SMDT.UtcNow;
            }

            status.IsRunning = true;
            //await _hubContext.Clients.All.TaskQueueStatusUpdate(await GetQueueStatus()).ConfigureAwait(false);
            await SendSMTasks();
        }

    }

    public async Task SetStop(Guid Id)
    {
        if (taskQueueStatuses.TryGetValue(Id, out SMTask? status))
        {
            status.StopTS = SMDT.UtcNow;
            status.IsRunning = false;
            //await _hubContext.Clients.All.TaskQueueStatusUpdate(await GetQueueStatus()).ConfigureAwait(false);
            await SendSMTasks();
        }
    }

    private async ValueTask QueueAsync(SMQueCommand command, CancellationToken cancellationToken = default)
    {

        BackgroundTaskQueueConfig bq = new() { Command = command, CancellationToken = cancellationToken };
        await QueueAsync(bq).ConfigureAwait(false);
    }

    private async ValueTask QueueAsync(SMQueCommand command, object entity, CancellationToken cancellationToken = default)
    {

        BackgroundTaskQueueConfig bq = new() { Command = command, Entity = entity, CancellationToken = cancellationToken };
        await QueueAsync(bq).ConfigureAwait(false);
    }

    private SMQueCommand lastSMQueCommand = new();
    private async ValueTask QueueAsync(BackgroundTaskQueueConfig workItem)
    {
        //No need to stack up the same task
        //if (lastSMQueCommand == workItem.Command)
        //{
        //    if (taskQueueStatuses.TryGetValue(workItem.Id, out TaskStatus? status))
        //    {
        //        if (!status.IsRunning)
        //        {
        //            return;
        //        }
        //    }

        //}

        lastSMQueCommand = workItem.Command;

        _ = taskQueueStatuses.TryAdd(workItem.Id, new SMTask(default)
        {
            Id = taskQueueStatuses.Count,
            Command = workItem.Command.ToString(),
            IsRunning = false
        });
        await SendSMTasks();

        //await _hubContext.Clients.All.TaskQueueStatusUpdate(await GetQueueStatus()).ConfigureAwait(false);
        await _queue.Writer.WriteAsync(workItem).ConfigureAwait(false);
        _logger.LogInformation("Added {workItem.command} to Queue", workItem.Command);

    }
}
