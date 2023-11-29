namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

public interface ISchedulesDirect
{
    void ResetCache(string command);

    Task<bool> AddLineup(string lineup, CancellationToken cancellationToken);

    Task<bool> RemoveLineup(string lineup, CancellationToken cancellationToken);

    Task<bool> SDSync(List<StationIdLineup> StationIdLineups, CancellationToken cancellationToken);

    Task<Countries?> GetCountries(CancellationToken cancellationToken);

    Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default);

    Task<bool> GetImageUrl(string programId, ImageData icon, CancellationToken cancellationToken);

    Task<LineupResult?> GetLineup(string lineup, CancellationToken cancellationToken);

    Task<List<LineupPreview>> GetLineupPreviews(CancellationToken cancellationToken);

    Task<List<Lineup>> GetLineups(CancellationToken cancellationToken);

    Task<List<Schedule>> GetSchedules(List<string> stationIds, CancellationToken cancellationToken);

    Task<List<SDProgram>> GetSDPrograms(List<string> programIds, CancellationToken cancellationToken);

    Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken);

    Task<List<Station>> GetStations(CancellationToken cancellationToken);

    Task<SDStatus> GetStatus(CancellationToken cancellationToken);

    Task<bool> GetSystemReady(CancellationToken cancellationToken);
}