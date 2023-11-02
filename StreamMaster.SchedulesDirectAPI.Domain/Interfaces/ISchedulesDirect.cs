namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

public interface ISchedulesDirect
{
    Task<Countries?> GetCountries(CancellationToken cancellationToken);
    Task<string> GetEpg(List<StationIdLineUp> stationIdLineUps, int maxRatings, CancellationToken cancellationToken);
    Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default);
    Task<bool> GetImageUrl(string source, string fileName, CancellationToken cancellationToken);
    Task<LineUpResult?> GetLineup(string lineUp, CancellationToken cancellationToken);
    Task<List<LineUpPreview>> GetLineUpPreviews(CancellationToken cancellationToken);
    Task<List<Lineup>> GetLineups(CancellationToken cancellationToken);
    Task<List<Schedule>?> GetSchedules(List<string> stationIds, CancellationToken cancellationToken);
    Task<List<SDProgram>> GetSDPrograms(List<string> programIds, CancellationToken cancellationToken);
    Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken);
    Task<List<Station>> GetStations(CancellationToken cancellationToken);
    Task<SDStatus> GetStatus(CancellationToken cancellationToken);
    Task<bool> GetSystemReady(CancellationToken cancellationToken);
}