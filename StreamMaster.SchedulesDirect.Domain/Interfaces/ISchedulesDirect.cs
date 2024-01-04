using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface ISchedulesDirect
{
    bool CheckToken(bool forceReset = false);
    Task<bool> AddLineup(string lineup, CancellationToken cancellationToken);
    Task<List<CountryData>?> GetAvailableCountries(CancellationToken cancellationToken);
    Task<List<string>?> GetCustomLogosFromServerAsync(string server);
    Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default);
    Task<List<LineupPreviewChannel>?> GetLineupPreviewChannel(string lineup, CancellationToken cancellationToken);
    Task<StationChannelMap?> GetStationChannelMapAsync(string lineup);
    Task<UserStatus> GetUserStatus(CancellationToken cancellationToken);
    Task<bool> RemoveLineup(string lineup, CancellationToken cancellationToken);
    void ResetCache(string command);
    void ResetEPGCache();
    Task<bool> SDSync(int EPGID, CancellationToken cancellationToken);

}