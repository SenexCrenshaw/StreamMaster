using StreamMaster.SchedulesDirectAPI.Domain.Interfaces;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStations : IRequest<List<Station>>;

internal class GetStationsHandler(ISDService sdService) : IRequestHandler<GetStations, List<Station>>
{
    public async Task<List<Station>> Handle(GetStations request, CancellationToken cancellationToken)
    {

        List<Station> ret = await sdService.GetStations(cancellationToken).ConfigureAwait(false);
        return ret;
    }
}
