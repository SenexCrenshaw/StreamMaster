
using StreamMaster.SchedulesDirectAPI.Domain.EPG;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;


public interface ISDService
{
    Task<List<Programme>> GetProgrammes(int maxDays, int maxRatings, CancellationToken cancellationToken);
    Task<List<Schedule>?> GetSchedules(List<string> stationsIds, CancellationToken cancellationToken);
    Task<List<SDProgram>> GetSDPrograms(List<string> progIds, CancellationToken cancellationToken);
    Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken);
    Task<List<Station>> GetStations(CancellationToken cancellationToken);
    Task<SDStatus> GetStatus(CancellationToken cancellationToken);
}
