using MediatR;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.Services;

namespace StreamMasterApplication.Settings.Queries;

public record GetQueueStatus : IRequest<List<TaskQueueStatusDto>>;

internal class GetQueueStatusHandler : IRequestHandler<GetQueueStatus, List<TaskQueueStatusDto>>
{
    private readonly IBackgroundTaskQueue _taskQueue;

    public GetQueueStatusHandler(
        IBackgroundTaskQueue taskQueue
        )
    {
        _taskQueue = taskQueue;
    }

    public async Task<List<TaskQueueStatusDto>> Handle(GetQueueStatus request, CancellationToken cancellationToken)
    {
        return await _taskQueue.GetQueueStatus().ConfigureAwait(false);
    }
}
