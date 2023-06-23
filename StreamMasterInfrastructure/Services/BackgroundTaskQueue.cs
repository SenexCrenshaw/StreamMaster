using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirect;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Hubs;
using StreamMasterApplication.Services;

using StreamMasterDomain.Enums;

using System.Threading.Channels;

namespace StreamMasterInfrastructure.Services;

public partial class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly IHubContext<StreamMasterHub, IStreamMasterHub> _hubContext;
    private readonly ILogger<BackgroundTaskQueue> _logger;
    private readonly Channel<BackgroundTaskQueueConfig> _queue;
    private readonly LinkedList<TaskQueueStatusDto> taskQueueStatusDtos = new();
    private readonly ISender _sender;
    private static object lockObject = new object();
    
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

    public Task<List<TaskQueueStatusDto>> GetQueueStatus()
    {
        return Task.FromResult(taskQueueStatusDtos.OrderBy(a=>a.StartTS).ToList());
    }

    public async ValueTask SetIsSystemReady(bool isSystemReady, CancellationToken cancellationToken = default)
    {
        await QueueAsync(SMQueCommand.SetIsSystemReady, isSystemReady, cancellationToken).ConfigureAwait(false);
    }

    public async Task SetQueueTS(Guid Id)
    {
        TaskQueueStatusDto status = null;
        lock (taskQueueStatusDtos)
        {
            status = taskQueueStatusDtos.FirstOrDefault(a => a.Id == Id);
            if (status != null)
            {
                status.QueueTS = DateTime.Now;
            status.IsRunning = true;
            }
        }
        if (status != null)
        {
            await _hubContext.Clients.All.TaskQueueStatusDtoesUpdate(taskQueueStatusDtos).ConfigureAwait(false);
        }
    }

    public async Task SetStart(Guid Id)
    {
        TaskQueueStatusDto status = null;
        lock (taskQueueStatusDtos)
        {
            status = taskQueueStatusDtos.FirstOrDefault(a => a.Id == Id);
            if (status != null)
            {
                status.StartTS = DateTime.Now;
                status.IsRunning = true;
            }
        }
        if (status != null)
        {
            await _hubContext.Clients.All.TaskQueueStatusDtoesUpdate(taskQueueStatusDtos).ConfigureAwait(false);
        }        
    }

    public async Task SetStop(Guid Id)
    {
        TaskQueueStatusDto status = null;
        lock (taskQueueStatusDtos) {
            status = taskQueueStatusDtos.FirstOrDefault(a => a.Id == Id);
            if (status != null)
            {
                status.StopTS = DateTime.Now;
        status.IsRunning = false;
            }
        }
        if (status != null)
        {
            await _hubContext.Clients.All.TaskQueueStatusDtoesUpdate(taskQueueStatusDtos).ConfigureAwait(false);
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

    private async ValueTask QueueAsync(BackgroundTaskQueueConfig workItem)
    {
        _logger.LogInformation("Added {workItem.command} to Queue", workItem.Command);

        _ = taskQueueStatusDtos.AddFirst(new TaskQueueStatusDto
        {
            Id = workItem.Id,
            Command = workItem.Command.ToString(),
        });

        await _hubContext.Clients.All.TaskQueueStatusDtoesUpdate(taskQueueStatusDtos).ConfigureAwait(false);
        lock (lockObject)
        {
            _queue.Writer.TryWrite(workItem);
            //await _queue.Writer.WriteAsync(workItem).ConfigureAwait(false);
        }
    }
}
