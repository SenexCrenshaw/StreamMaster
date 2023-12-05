using  StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetAvailableCountries : IRequest<List<CountryData>?>;

internal class GetAvailableCountriesHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetAvailableCountries, List<CountryData>?>
{

    public  async Task<List<CountryData>?> Handle(GetAvailableCountries request, CancellationToken cancellationToken)
    {
        var countries = await schedulesDirect.GetAvailableCountries(cancellationToken);

        return countries ?? [];
    }
}
