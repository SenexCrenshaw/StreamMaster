namespace StreamMasterApplication.SchedulesDirect.Queries;

public record GetUserStatus : IRequest<UserStatus>;

internal class GetStatusHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetUserStatus, UserStatus>
{
    public async Task<UserStatus> Handle(GetUserStatus request, CancellationToken cancellationToken)
    {
        UserStatus status = await schedulesDirect.GetUserStatus(cancellationToken).ConfigureAwait(false);
        return status ?? new UserStatus();
    }
}
