using MediatR;

using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetCountries : IRequest<Countries?>;

internal class GetCountriesHandler : IRequestHandler<GetCountries, Countries?>
{
    public async Task<Countries?> Handle(GetCountries request, CancellationToken cancellationToken)
    {
        var sd = new SchedulesDirect();
        var status = await sd.GetStatus(cancellationToken).ConfigureAwait(false);
        if (status == null || !status.systemStatus.Any())
        {
            Console.WriteLine("Status is null");
            return null;
        }

        var systemStatus = status.systemStatus[0];
        if (systemStatus.status == "Offline")
        {
            Console.WriteLine($"Status is {systemStatus.status}");
            return null;
        }

        var ret = await sd.GetCountries(cancellationToken).ConfigureAwait(false);

        return ret;
    }
}
