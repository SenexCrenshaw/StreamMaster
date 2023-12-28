using StreamMaster.Application.Common.Models;
using StreamMaster.Application.Services;

namespace StreamMaster.Application.Settings.Queries;

public record GetQueueStatus : IRequest<List<TaskQueueStatus>>;

internal class GetQueueStatusHandler : IRequestHandler<GetQueueStatus, List<TaskQueueStatus>>
{
    private readonly IBackgroundTaskQueue _taskQueue;

    public GetQueueStatusHandler(
        IBackgroundTaskQueue taskQueue
        )
    {
        _taskQueue = taskQueue;
    }

    public async Task<List<TaskQueueStatus>> Handle(GetQueueStatus request, CancellationToken cancellationToken)
    {
        return await _taskQueue.GetQueueStatus().ConfigureAwait(false);
    }
}
