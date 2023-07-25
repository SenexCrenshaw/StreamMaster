using StreamMasterApplication.Common.Models;

namespace StreamMasterApplication.Services;

public interface IBackgroundTaskQueue : ISharedTasks
{
    ValueTask<BackgroundTaskQueueConfig> DeQueueAsync(
        CancellationToken cancellationToken);

    Task<List<TaskQueueStatusDto>> GetQueueStatus();

    bool IsCurrent();

    Task SetStart(Guid Id);

    Task SetStop(Guid Id);

    //ValueTask QueueAsync(BackgroundTaskQueueConfig workItem);
}
