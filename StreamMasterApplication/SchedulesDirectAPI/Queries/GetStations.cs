using MediatR;

using StreamMaster.SchedulesDirectAPI;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetStations : IRequest<List<Station>>;

internal class GetStationsHandler : IRequestHandler<GetStations, List<Station>>
{
    public async Task<List<Station>> Handle(GetStations request, CancellationToken cancellationToken)
    {
        var ret = new List<Station>();
        var sd = new SchedulesDirect();

        ret = await sd.GetStations(cancellationToken).ConfigureAwait(false);


        return ret;
    }
}
