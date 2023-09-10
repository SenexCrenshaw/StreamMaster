using MediatR;

using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStatus : IRequest<SDStatus>;

internal class GetStatusHandler : IRequestHandler<GetStatus, SDStatus>
{
    public async Task<SDStatus> Handle(GetStatus request, CancellationToken cancellationToken)
    {
        var sd = new SchedulesDirect();
        var status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        return status ?? new SDStatus();
    }
}
