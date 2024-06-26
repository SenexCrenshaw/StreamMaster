namespace StreamMaster.Application.SchedulesDirect.QueriesOld;

public record GetUserStatusRequest : IRequest<DataResponse<UserStatus>>;

internal class GetStatusHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetUserStatusRequest, DataResponse<UserStatus>>
{
    public async Task<DataResponse<UserStatus>> Handle(GetUserStatusRequest request, CancellationToken cancellationToken)
    {
        UserStatus status = await schedulesDirect.GetUserStatus(cancellationToken).ConfigureAwait(false);
        return DataResponse<UserStatus>.Success(status ?? new UserStatus());
    }
}
