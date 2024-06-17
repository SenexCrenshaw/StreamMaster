using StreamMaster.Application.Services;

namespace StreamMaster.Application.Queue.Queries;

public record GetQueueStatus : IRequest<List<SMTask>>;

internal class GetQueueStatusHandler(
    IBackgroundTaskQueue taskQueue
        ) : IRequestHandler<GetQueueStatus, List<SMTask>>
{
    public async Task<List<SMTask>> Handle(GetQueueStatus request, CancellationToken cancellationToken)
    {
        List<SMTask> data = await taskQueue.GetQueueStatus().ConfigureAwait(false);
        return data.OrderBy(a => a.Id).ToList();
    }
}
