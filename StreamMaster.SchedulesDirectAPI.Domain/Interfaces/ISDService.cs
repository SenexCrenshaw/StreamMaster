
using StreamMaster.SchedulesDirectAPI.Domain.EPG;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;


public interface ISDService
{
    Task<List<Lineup>> GetLineups(CancellationToken cancellationToken);
    Task<List<LineUpPreview>> GetLineUpPreviews(CancellationToken cancellationToken);
    Task<LineUpResult?> GetLineup(string lineUp, CancellationToken cancellationToken);
    Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default);
    Task<string> GetEpg(CancellationToken cancellationToken);
    Task<Countries?> GetCountries(CancellationToken cancellationToken);
    Task SDSync(CancellationToken cancellationToken);
    Task<List<Programme>> GetProgrammes(int maxDays, int maxRatings, bool useLineUpInName, CancellationToken cancellationToken);
    Task<List<Schedule>?> GetSchedules(List<string> stationsIds, CancellationToken cancellationToken);
    Task<List<SDProgram>> GetSDPrograms(List<string> progIds, CancellationToken cancellationToken);
    Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken);
    Task<List<Station>> GetStations(CancellationToken cancellationToken);
    Task<SDStatus> GetStatus(CancellationToken cancellationToken);
}
