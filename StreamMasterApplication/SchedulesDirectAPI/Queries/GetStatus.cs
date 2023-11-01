using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStatus : IRequest<SDStatus>;

internal class GetStatusHandler(ISDService SDService) : IRequestHandler<GetStatus, SDStatus>
{

    public async Task<SDStatus> Handle(GetStatus request, CancellationToken cancellationToken)
    {
        SDStatus status = await SDService.GetStatus(cancellationToken).ConfigureAwait(false);
        return status ?? new SDStatus();
    }
}
