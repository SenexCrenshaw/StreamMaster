using StreamMaster.Domain.API;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface ISchedulesDirect
{
    void ClearAllCaches();
    bool CheckToken(bool forceReset = false);
    Task<int> AddLineup(string lineup, CancellationToken cancellationToken);
    Task<List<CountryData>?> GetAvailableCountries(CancellationToken cancellationToken);
    Task<List<string>?> GetCustomLogosFromServerAsync(string server);
    Task<List<Headend>?> GetHeadendsByCountryPostal(string country, string postalCode, CancellationToken cancellationToken = default);
    Task<List<LineupPreviewChannel>?> GetLineupPreviewChannel(string lineup, CancellationToken cancellationToken);
    Task<StationChannelMap?> GetStationChannelMapAsync(string lineup);
    Task<UserStatus> GetUserStatus(CancellationToken cancellationToken);
    Task<int> RemoveLineup(string lineup, CancellationToken cancellationToken);
    void ResetCache(string command);
    void ResetEPGCache();
    Task<APIResponse> SDSync(CancellationToken cancellationToken);
    Task<bool> UpdateHeadEnd(string lineup, bool subScribed, CancellationToken cancellationToken);
}