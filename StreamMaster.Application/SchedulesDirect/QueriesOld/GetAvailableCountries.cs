namespace StreamMaster.Application.SchedulesDirect.QueriesOld;

public record GetAvailableCountries : IRequest<List<CountryData>?>;

internal class GetAvailableCountriesHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetAvailableCountries, List<CountryData>?>
{

    public async Task<List<CountryData>?> Handle(GetAvailableCountries request, CancellationToken cancellationToken)
    {
        List<CountryData>? countries = await schedulesDirect.GetAvailableCountries(cancellationToken);

        return countries ?? [];
    }
}
