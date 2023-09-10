using MediatR;

using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetLineups : IRequest<LineUpsResult?>;

internal class GetLineupsHandler : IRequestHandler<GetLineups, LineUpsResult?>
{
    public async Task<LineUpsResult?> Handle(GetLineups request, CancellationToken cancellationToken)
    {
        var sd = new SchedulesDirect();
        var isReady = await sd.GetSystemReady(cancellationToken).ConfigureAwait(false);
        if (!isReady)
        {
            Console.WriteLine($"Status is Offline");
            return null;
        }

        var ret = await sd.GetLineups(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
