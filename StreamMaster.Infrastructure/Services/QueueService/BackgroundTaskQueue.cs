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
    private readonly SemaphoreSlim _queueSemaphore = new(1, 1); // Semaphore to ensure thread safety

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
        _logger.LogInformation("Got {command} from Queue", workItem.Command);
        return workItem;
    }

    public bool IsRunning => taskQueueStatuses.Values.Any(a => a.IsRunning || a.StopTS == DateTime.MinValue);

    public Task<List<SMTask>> GetQueueStatus()
    {
        return Task.FromResult(taskQueueStatuses.Values.OrderBy(a => a.StartTS).ToList());
    }

    public bool HasJobs() => taskQueueStatuses.Values.Any(a => a.StopTS == DateTime.MinValue);

    public async ValueTask SetIsSystemReady(bool isSystemReady, CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.SetIsSystemReady, isSystemReady, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask SetTestTask(int delayInSeconds, CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.SetTestTask, delayInSeconds, cancellationToken).ConfigureAwait(false);
    }

    public List<SMTask> GetSMTasks() => taskQueueStatuses.Values.OrderByDescending(a => a.Id).ToList();

    private async ValueTask SendSMTasks()
    {
        await dataRefreshService.RefreshSMTasks(true).ConfigureAwait(false);
        BuildInfo.IsTaskRunning = IsRunning;
        await dataRefreshService.TaskIsRunning().ConfigureAwait(false);
    }

    public async Task SetStart(Guid id)
    {
        if (taskQueueStatuses.TryGetValue(id, out SMTask? status))
        {
            status.StartTS = status.Command == "SetIsSystemReady" ? BuildInfo.StartTime : SMDT.UtcNow;
            status.IsRunning = true;
            await SendSMTasks().ConfigureAwait(false);
        }
    }

    public async Task SetStop(Guid id)
    {
        if (taskQueueStatuses.TryGetValue(id, out SMTask? status))
        {
            status.StopTS = SMDT.UtcNow;
            status.IsRunning = false;
            await SendSMTasks().ConfigureAwait(false);
        }
    }

    private async ValueTask QueueAsync(SMQueCommand command, CancellationToken cancellationToken = default)
    {
        await QueueAsync(new BackgroundTaskQueueConfig { Command = command, CancellationToken = cancellationToken }).ConfigureAwait(false);
    }

    private async ValueTask QueueAsync(SMQueCommand command, object entity, CancellationToken cancellationToken = default)
    {
        await QueueAsync(new BackgroundTaskQueueConfig { Command = command, Entity = entity, CancellationToken = cancellationToken }).ConfigureAwait(false);
    }

    private async ValueTask QueueAsync(BackgroundTaskQueueConfig workItem)
    {
        await _queueSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            // Avoid stacking up the same task if the previous one is not yet running
            if (taskQueueStatuses.TryGetValue(workItem.Id, out SMTask? existingTask) && !existingTask.IsRunning)
            {
                _logger.LogWarning("Duplicate task {command} not queued", workItem.Command);
                return;
            }

            taskQueueStatuses.TryAdd(workItem.Id, new SMTask(default)
            {
                Id = taskQueueStatuses.Count,
                Command = workItem.Command.ToString(),
                IsRunning = false
            });

            await SendSMTasks().ConfigureAwait(false);
            await _queue.Writer.WriteAsync(workItem).ConfigureAwait(false);
            _logger.LogInformation("Added {command} to Queue", workItem.Command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue {command}", workItem.Command);
        }
        finally
        {
            _queueSemaphore.Release();
        }
    }
}