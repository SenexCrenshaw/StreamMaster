using StreamMaster.Application.Common.Models;

namespace StreamMaster.Application.Services;

public interface IBackgroundTaskQueue : ISharedTasks
{
    ValueTask<BackgroundTaskQueueConfig> DeQueueAsync(CancellationToken cancellationToken);
    Task<List<SMTask>> GetQueueStatus();
    bool HasJobs();
    Task SetStart(Guid Id);
    Task SetStop(Guid Id);
    List<SMTask> GetSMTasks();
}
