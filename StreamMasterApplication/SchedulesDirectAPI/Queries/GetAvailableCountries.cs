using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterApplication.SchedulesDirectAPI.Queries;

public record GetAvailableCountries : IRequest<Dictionary<string, List<Country>>>;

internal class GetAvailableCountriesHandler(ISchedulesDirect schedulesDirect) : IRequestHandler<GetAvailableCountries, Dictionary<string, List<Country>>>
{

    public  async Task<Dictionary<string, List<Country>>> Handle(GetAvailableCountries request, CancellationToken cancellationToken)
    {
        var countries = await schedulesDirect.GetAvailableCountries(cancellationToken);

        return countries ?? [];
    }
}
