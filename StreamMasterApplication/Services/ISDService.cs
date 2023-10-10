using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

using StreamMasterDomain.EPG;

namespace StreamMasterApplication.Services;

public interface ISDService
{
    Task<List<Programme>> GetProgrammes(CancellationToken cancellationToken);
    Task<List<Schedule>?> GetSchedules(List<string> stationsIds, CancellationToken cancellationToken);
    Task<List<SDProgram>> GetSDPrograms(List<string> progIds, CancellationToken cancellationToken);
    Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken);
    Task<List<Station>> GetStations(CancellationToken cancellationToken);
}
