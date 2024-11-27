namespace StreamMaster.Application.SchedulesDirect.Commands;

public record EPGRemovedExpiredKeysRequest() : IRequest<APIResponse>;

public class EPGRemovedExpiredKeysRequestHandler(ISchedulesDirect schedulesDirect)
: IRequestHandler<EPGRemovedExpiredKeysRequest, APIResponse>
{
    public Task<APIResponse> Handle(EPGRemovedExpiredKeysRequest request, CancellationToken cancellationToken)
    {
        schedulesDirect.RemovedExpiredKeys();

        return Task.FromResult(APIResponse.Ok);
    }
}