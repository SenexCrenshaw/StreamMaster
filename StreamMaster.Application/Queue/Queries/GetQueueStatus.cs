using StreamMaster.Application.Common.Models;
using StreamMaster.Application.Services;

namespace StreamMaster.Application.Queue.Queries;

public record GetQueueStatus : IRequest<List<TaskQueueStatus>>;

internal class GetQueueStatusHandler(
    IBackgroundTaskQueue taskQueue
        ) : IRequestHandler<GetQueueStatus, List<TaskQueueStatus>>
{
    public async Task<List<TaskQueueStatus>> Handle(GetQueueStatus request, CancellationToken cancellationToken)
    {
        return await taskQueue.GetQueueStatus().ConfigureAwait(false);
    }
}
