using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;

using StreamMasterApplication.Common.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;

using StreamMasterDomain.Services;

namespace StreamMaster.SchedulesDirectAPI.Services;

[LogExecutionTimeAspect]
public class SDService(IMemoryCache memoryCache, ILogger<SDService> logger, ISettingsService settingsService, ISchedulesDirect sd) : ISDService
{
    [LogExecutionTimeAspect]
    public async Task<bool> SDSync(CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return false;
        }

        return await sd.SDSync(setting.SDStationIds, cancellationToken);
    }

    [LogExecutionTimeAspect]
    public List<Programme> GetProgrammes()
    {
        return memoryCache.SDProgrammess();
    }

    public async Task<Countries?> GetCountries(CancellationToken cancellationToken)
    {
        return await sd.GetCountries(cancellationToken);
    }

    public async Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken)
    {
        return await sd.GetStationPreviews(cancellationToken);
    }

    public async Task<bool> AddLineup(string lineup, CancellationToken cancellationToken)
    {
        return await sd.AddLineup(lineup, cancellationToken);
    }

    public async Task<bool> RemoveLineup(string lineup, CancellationToken cancellationToken)
    {
        return await sd.RemoveLineup(lineup, cancellationToken);
    }

    public async Task<List<Schedule>?> GetSchedules(List<string> stationsIds, CancellationToken cancellationToken)
    {
        return await sd.GetSchedules(stationsIds, cancellationToken);
    }

    public async Task<List<SDProgram>> GetSDPrograms(List<string> progIds, CancellationToken cancellationToken)
    {
        return await sd.GetSDPrograms(progIds, cancellationToken);
    }

    public void ResetCache(string command)
    {
        sd.ResetCache(command);
    }

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        return await sd.GetStations(cancellationToken);
    }

    public async Task<SDStatus> GetStatus(CancellationToken cancellationToken)
    {
        return await sd.GetStatus(cancellationToken);
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        return await sd.GetHeadends(country, postalCode, cancellationToken);
    }

    public async Task<LineupResult?> GetLineup(string lineup, CancellationToken cancellationToken)
    {
        return await sd.GetLineup(lineup, cancellationToken);
    }

    public async Task<List<LineupPreview>> GetLineupPreviews(CancellationToken cancellationToken)
    {
        return await sd.GetLineupPreviews(cancellationToken);
    }

    public async Task<List<Lineup>> GetLineups(CancellationToken cancellationToken)
    {
        return await sd.GetLineups(cancellationToken);
    }
}