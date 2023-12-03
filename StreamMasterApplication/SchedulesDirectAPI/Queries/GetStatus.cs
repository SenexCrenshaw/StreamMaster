using StreamMaster.SchedulesDirectAPI;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStatus : IRequest<UserStatus>;

internal class GetStatusHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetStatus, UserStatus>
{

    public async Task<UserStatus> Handle(GetStatus request, CancellationToken cancellationToken)
    {
        UserStatus status = await schedulesDirect.GetStatus(cancellationToken).ConfigureAwait(false);
        return status ?? new UserStatus();
    }
}
