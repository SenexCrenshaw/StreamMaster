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
        List<TaskQueueStatus> data = await taskQueue.GetQueueStatus().ConfigureAwait(false);
        return data.OrderBy(a => a.Id).ToList();
    }
}
