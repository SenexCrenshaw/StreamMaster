using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain.EPG;
using StreamMaster.SchedulesDirectAPI.Helpers;

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
    public async Task<string> GetEpg(CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);
        List<StationIdLineup> stationIdLineups = setting.SDStationIds;
        List<string> stationsIds = stationIdLineups.ConvertAll(a => a.StationId).Distinct().ToList();

        await sd.SDSync(stationIdLineups, cancellationToken);

        List<Schedule>? schedules = await GetSchedules(stationsIds, cancellationToken);

        if (schedules?.Any() != true)
        {
            Console.WriteLine("No schedules");
            return FileUtil.SerializeEpgData(new Tv());
        }

        List<TvChannel> retChannels = new();
        List<Programme> retProgrammes = new();

        List<Station> stations = await GetStations(cancellationToken).ConfigureAwait(false);

        foreach (string? stationId in stationsIds)
        {
            List<string> names = stations.Where(a => a.StationId == stationId).Select(a => a.Name).Distinct().ToList();
            List<List<StationLogo>> logos = stations.Where(a => a.StationId == stationId).Select(a => a.StationLogo).Distinct().ToList();
            ILogo? logo = stations.Where(a => a.StationId == stationId).Select(a => a.Logo).FirstOrDefault();
            IStation station = stations.First(a => a.StationId == stationId);

            TvChannel channel = new()
            {
                Id = stationId,
                Displayname = names,
            };

            if (station.Logo != null)
            {
                channel.Icon = new TvIcon
                {
                    Src = station.Logo.URL
                };
            }

            retChannels.Add(channel);
        }

        List<string> progIds = schedules.SelectMany(a => a.Programs).Select(a => a.ProgramID).Distinct().ToList();
        List<SDProgram> programs = await GetSDPrograms(progIds, cancellationToken).ConfigureAwait(false);

        foreach (SDProgram sdProg in programs)
        {
            foreach (Schedule sched in schedules.Where(a => a.Programs.Any(a => a.ProgramID == sdProg.ProgramID)).ToList())
            {
                IStation station = stations.First(a => a.StationId == sched.StationID);

                foreach (Program p in sched.Programs)
                {
                    DateTime startt = p.AirDateTime;
                    DateTime endt = startt.AddSeconds(p.Duration);
                    string lang = "en";
                    if (station.BroadcastLanguage.Any())
                    {
                        lang = station.BroadcastLanguage[0];
                    }

                    Programme programme = new()
                    {
                        Start = startt.ToString("yyyyMMddHHmmss") + " +0000",
                        Stop = endt.ToString("yyyyMMddHHmmss") + " +0000",
                        Channel = sched.StationID,

                        Title = SDHelpers.GetTitles(sdProg.Titles, lang),
                        Subtitle = SDHelpers.GetSubTitles(sdProg, lang),
                        Desc = SDHelpers.GetDescriptions(sdProg, lang),
                        Credits = SDHelpers.GetCredits(sdProg, lang),
                        Category = SDHelpers.GetCategory(sdProg, lang),
                        Language = lang,
                        Episodenum = SDHelpers.GetEpisodeNums(sdProg, lang),
                        Icon = SDHelpers.GetIcons(p, sdProg, sched, lang),
                        Rating = SDHelpers.GetRatings(sdProg, lang, setting.SDMaxRatings),
                        Video = SDHelpers.GetTvVideos(p),
                        Audio = SDHelpers.GetTvAudios(p),
                    };

                    if (p.New is not null)
                    {
                        programme.New = ((bool)p.New).ToString();
                    }

                    if (!string.IsNullOrEmpty(p.LiveTapeDelay) && p.LiveTapeDelay.Equals("Live"))
                    {
                        programme.Live = "";
                    }

                    retProgrammes.Add(programme);
                }
            }
        }

        Tv tv = new()
        {
            Channel = retChannels.OrderBy(a => int.Parse(a!.Id)).ToList(),
            Programme = retProgrammes.OrderBy(a => int.Parse(a.Channel)).ThenBy(a => a.StartDateTime).ToList()
        };
        return FileUtil.SerializeEpgData(tv);
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